using System;
using System.Data.Objects;

using NCommon.Data.EntityFramework.Tests.HRDomain;
using NCommon.Data.EntityFramework.Tests.OrdersDomain;
using NCommon.StateStorage;
using NCommon.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.EntityFramework.Tests
{
    public abstract class EFRepositoryTestBase
    {
        protected IState State { get; private set; }
        protected IServiceLocator Locator { get; private set; }
        protected Func<ObjectContext> OrdersContextProvider { get; private set; }
        protected Func<ObjectContext> HRContextProvider { get; private set; }
        protected EFUnitOfWorkFactory UnitOfWorkFactory { get; private set; }

        [TestFixtureSetUp()]
        public virtual void FixtureSetup()
        {
            OrdersContextProvider = () =>
            {
                var orderEntities = new OrderEntities();
                orderEntities.ContextOptions.LazyLoadingEnabled = true;
                return orderEntities;
            };

            HRContextProvider = () =>
            {
                var hrEntities = new HREntities();
                hrEntities.ContextOptions.LazyLoadingEnabled = true;
                return hrEntities;
            };

            UnitOfWorkFactory = new EFUnitOfWorkFactory();
            UnitOfWorkFactory.RegisterObjectContextProvider(HRContextProvider);
            UnitOfWorkFactory.RegisterObjectContextProvider(OrdersContextProvider);

            Locator = MockRepository.GenerateStub<IServiceLocator>();
            Locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(UnitOfWorkFactory);
            Locator.Stub(x => x.GetInstance<IState>()).Do(new Func<IState>(() => State));
            ServiceLocator.SetLocatorProvider(() => Locator);
        }

        [SetUp()]
        public virtual void TestSetup()
        {
            State = new FakeState();
        }
    }
}