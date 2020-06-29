using System;
using System.Transactions;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

using NCommon.DataServices.Transactions;
using NCommon.Data.NHibernate.Tests.HRDomain.Domain;
using NCommon.Data.NHibernate.Tests.OrdersDomain;
using NCommon.StateStorage;
using NCommon.Testing;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using Rhino.Mocks;
using CommonServiceLocator;

namespace NCommon.Data.NHibernate.Tests
{
    public abstract class NHRepositoryTestBase
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
                .ExposeConfiguration(config =>
                {
                    var export = new SchemaExport(config);
                    export.Drop(false, true);
                    export.Create(false, true);
                })
                .BuildSessionFactory();

            HRDomainFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2005
                            .ConnectionString(x => x.FromConnectionStringWithKey("testdb")))
                .Mappings(mappings => mappings.FluentMappings.AddFromAssemblyOf<SalesPerson>())
                .ExposeConfiguration(config =>
                {
                    var export = new SchemaExport(config);
                    export.Drop(false, true);
                    export.Create(false, true);
                })
                .BuildSessionFactory();

            UnitOfWorkFactory = new NHUnitOfWorkFactory();
            UnitOfWorkFactory.RegisterSessionFactoryProvider(() => OrdersDomainFactory);
            UnitOfWorkFactory.RegisterSessionFactoryProvider(() => HRDomainFactory);

            Locator = MockRepository.GenerateStub<IServiceLocator>();
            Locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(UnitOfWorkFactory);
            Locator.Stub(x => x.GetInstance<IState>()).Do(new Func<IState>(() => State));
            UnitOfWorkSettings.DefaultIsolation = IsolationLevel.ReadCommitted;

            ServiceLocator.SetLocatorProvider(() => Locator);
        }

        [SetUp]
        public virtual void TestSetup()
        {
            State = new FakeState();
        }
    }
}