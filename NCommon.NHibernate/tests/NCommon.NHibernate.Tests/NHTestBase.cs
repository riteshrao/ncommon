using System;
using System.Configuration;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Practices.ServiceLocation;
using NCommon.Data.NHibernate.Tests.HRDomain.Domain;
using NCommon.Data.NHibernate.Tests.OrdersDomain;
using NCommon.State;
using NCommon.Util;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.NHibernate.Tests
{
	public class NHTestBase
	{
        protected IState State { get; private set; }
		protected ISessionFactory OrdersDomainFactory { get; private set; }
        protected ISessionFactory HRDomainFactory { get; private set; }
        protected IServiceLocator Locator { get; private set; }
        protected NHUnitOfWorkFactory UnitOfWorkFactory { get; private set; }

		/// <summary>
		/// Sets up the NHibernate SessionFactory and NHUnitOfWorkFactory.
		/// </summary>
		[TestFixtureSetUp]
		public virtual void SetUp()
		{
		    OrdersDomainFactory = Fluently.Configure()
		        .Database(MsSqlConfiguration.MsSql2005
		                      .ConnectionString(x => x.FromConnectionStringWithKey("testdb")))
		        .Mappings(mappings => mappings.FluentMappings.AddFromAssemblyOf<Order>())
                .ExposeConfiguration(config => new SchemaUpdate(config).Execute(false, true))
		        .BuildSessionFactory();

            HRDomainFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2005
                            .ConnectionString(x => x.FromConnectionStringWithKey("testdb")))
                .Mappings(mappings => mappings.FluentMappings.AddFromAssemblyOf<Employee>())
                .ExposeConfiguration(config => new SchemaUpdate(config).Execute(false, true))
                .BuildSessionFactory();

            UnitOfWorkFactory = new NHUnitOfWorkFactory();
		    UnitOfWorkFactory.RegisterSessionFactoryProvider(() => OrdersDomainFactory);
		    UnitOfWorkFactory.RegisterSessionFactoryProvider(() => HRDomainFactory);

			Locator = MockRepository.GenerateStub<IServiceLocator>();
			Locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(UnitOfWorkFactory);
            Locator.Stub(x => x.GetInstance<IState>()).Do(new Func<IState>(() => State));

			ServiceLocator.SetLocatorProvider(() => Locator);
		}

        [SetUp]
        public virtual void TestSetup ()
        {
            State = new FakeState();
        }
	}
}