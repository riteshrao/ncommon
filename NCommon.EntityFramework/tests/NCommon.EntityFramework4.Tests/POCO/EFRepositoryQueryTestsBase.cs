using System;
using System.Configuration;

using NCommon.DataServices.Transactions;
using NCommon.Data.EntityFramework;
using NCommon.StateStorage;
using NCommon.Testing;
using NUnit.Framework;
using Rhino.Mocks;
using CommonServiceLocator;

namespace NCommon.EntityFramework4.Tests.POCO
{
    public class EFRepositoryQueryTestsBase
    {
        protected IState State;
        protected string ConnectionString;
        protected IServiceLocator Locator;
        protected PocoContext Context;
        EFUnitOfWorkFactory _unitOfWorkFactory;

        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
            _unitOfWorkFactory = new EFUnitOfWorkFactory();
            ConnectionString = ConfigurationManager.ConnectionStrings["Sandbox"].ConnectionString;
            _unitOfWorkFactory.RegisterObjectContextProvider(() => new PocoContext(ConnectionString));

            Locator = MockRepository.GenerateStub<IServiceLocator>();
            Locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(_unitOfWorkFactory);
            Locator.Stub(x => x.GetInstance<IState>()).Do(new Func<IState>(() => State));
            ServiceLocator.SetLocatorProvider(() => Locator);
        }

        [SetUp]
        public virtual void TestSetup()
        {
            State = new FakeState();
            Context = new PocoContext(ConnectionString);
        }

        [TearDown]
        public void TestTeardown()
        {
            Context = new PocoContext(ConnectionString);
            Context.ExecuteStoreCommand("DELETE OrderItems");
            Context.ExecuteStoreCommand("DELETE Products");
            Context.ExecuteStoreCommand("DELETE Orders");
            Context.ExecuteStoreCommand("DELETE Customers");
        }
    }
}