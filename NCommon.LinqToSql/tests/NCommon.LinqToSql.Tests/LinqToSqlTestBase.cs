using System;
using System.Configuration;
using System.Data.Linq;
using Microsoft.Practices.ServiceLocation;
using NCommon.Data.LinqToSql.Tests.HRDomain;
using NCommon.Data.LinqToSql.Tests.OrdersDomain;
using NCommon.State;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.LinqToSql.Tests
{
    public abstract class LinqToSqlTestBase
    {
        protected IState State { get; private set; }
        protected IServiceLocator Locator { get; private set; }
        protected Func<DataContext> OrdersContextProvider { get; private set; }
        protected Func<DataContext> HRContextProvider { get; private set; }
        protected LinqToSqlUnitOfWorkFactory UnitOfWorkFactory { get; private set; }

        [TestFixtureSetUp()]
        public virtual void FixtureSetup()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DevelopmentDBConnectionString"].ConnectionString;
            OrdersContextProvider = () => new OrdersDataDataContext(connectionString);
            HRContextProvider = () => new HRDataDataContext(connectionString);
            UnitOfWorkFactory = new LinqToSqlUnitOfWorkFactory();
            UnitOfWorkFactory.RegisterDataContextProvider(OrdersContextProvider);
            UnitOfWorkFactory.RegisterDataContextProvider(HRContextProvider);

            Locator = MockRepository.GenerateStub<IServiceLocator>();
            Locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(UnitOfWorkFactory);
            Locator.Stub(x => x.GetInstance<IState>()).Do(new Func<IState>(() => State));
            ServiceLocator.SetLocatorProvider(() => Locator);
        }

        [SetUp]
        public virtual void TestSetup()
        {
            State = new FakeState();
        }
    }
}