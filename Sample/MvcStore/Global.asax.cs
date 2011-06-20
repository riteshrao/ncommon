using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Practices.ServiceLocation;
using MvcStore.Services;
using NCommon.Configuration;
using NCommon.ContainerAdapter.CastleWindsor;
using NCommon.Data.NHibernate;
using NHibernate;
using NHibernate.ByteCode.Castle;
using NHibernate.Tool.hbm2ddl;

namespace MvcStore
{
    public class MvcApplication : System.Web.HttpApplication
    {
        static bool _configured;
        static IWindsorContainer _container;
        static IServiceLocator _serviceLocator;
        static ISessionFactory _sessionFactory;
        static object _configureLock = new object();

        protected void Application_Start()
        {
            Configure();
        }

        protected void Begin_Request()
        {
            Configure();
        }

        void Configure()
        {
            if (_configured)
                return;
            lock(_configureLock)
            {
                if (_configured)
                    return;
                ConfigureContainer();
                ConfigureNHibernate();
                ConfigureNCommon();
                _configured = true;
            }
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(_container));
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
        }

        static void ConfigureContainer()
        {
            _container = new WindsorContainer();
            _container.Register(
                AllTypes.FromAssemblyContaining<MvcApplication>()
                    .Where(x => x.Namespace.Contains("Services"))
                    .WithService.FirstInterface()
                    .Configure(c => c.LifeStyle.Is(LifestyleType.Singleton)),
                AllTypes.FromAssemblyContaining<MvcApplication>()
                    .BasedOn<Controller>()
                    .Configure(c =>
                    {
                        c.Named(c.Implementation.Name.Replace("Controller", ""));
                        c.LifeStyle.Is(LifestyleType.Transient);
                    })
                );
            _serviceLocator = (IServiceLocator) new WindsorServiceLocator(_container);
            ServiceLocator.SetLocatorProvider(() => _serviceLocator);
        }

        static void ConfigureNHibernate()
        {
            var schemaMode = ConfigurationManager.AppSettings["SchemaExportMode"];
            var configuration = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008
                              .ConnectionString(x => x.FromConnectionStringWithKey("MvcStoreDb"))
                              .ProxyFactoryFactory<ProxyFactoryFactory>())
                .Mappings(mappings => mappings.FluentMappings.AddFromAssemblyOf<MvcApplication>())
                .BuildConfiguration();
            _sessionFactory = configuration.BuildSessionFactory();

            switch (schemaMode)
            {
                case ("CREATE"):
                    var storeInitializer = ServiceLocator.Current.GetInstance<IStoreInitializer>();
                    var export = new SchemaExport(configuration);
                    export.Drop(false, true);
                    export.Create(false, true);
                    storeInitializer.Initialize(_sessionFactory);
                    break;
                case ("UPDATE"):
                    new SchemaUpdate(configuration).Execute(false, true);
                    break;
            }
        }

        static void ConfigureNCommon()
        {
            var adapter = new WindsorContainerAdapter(_container);
            NCommon.Configure.Using(adapter)
                .ConfigureState<DefaultStateConfiguration>()
                .ConfigureData<NHConfiguration>(config => config.WithSessionFactory(() => _sessionFactory))
                .ConfigureUnitOfWork<DefaultUnitOfWorkConfiguration>(config => config.AutoCompleteScope());
        }

        static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ico");
            routes.MapRoute(
                "Default",
                "{controller}/{action}", 
                new { controller = "Store", action = "Index"});

        }
    }
}