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
    public abstract class LinqToSqlRepositoryTestBase
    {
        protected IState State { get; private set; }
        protected IServiceLocator Locator { get; private set; }
        protected Func<DataContext> OrdersContextProvider { get; private set; }
        protected Func<DataContext> HRContextProvider { get; private set; }
        protected LinqToSqlUnitOfWorkFactory UnitOfWorkFactory { get; private set; }

        [TestFixtureSetUp()]
        public virtual void FixtureSetup()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["testDb"].ConnectionString;
            OrdersContextProvider = () =>
            {
                var ordersDataDataContext = new OrdersDataDataContext(connectionString);
                ordersDataDataContext.DeferredLoadingEnabled = true;
                return ordersDataDataContext;
            };
            HRContextProvider = () =>
            {
                var hrDataDataContext = new HRDataDataContext(connectionString);
                hrDataDataContext.DeferredLoadingEnabled = true;
                return hrDataDataContext;
            };

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