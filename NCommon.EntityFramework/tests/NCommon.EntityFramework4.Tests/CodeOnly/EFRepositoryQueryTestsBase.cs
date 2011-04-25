using System;
using System.Data.Entity;
using System.Data.Objects;
using Microsoft.Practices.ServiceLocation;
using NCommon.Data;
using NCommon.Data.EntityFramework;
using NCommon.State;
using NCommon.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.EntityFramework4.Tests.CodeOnly
{
    public class EFRepositoryQueryTestsBase
    {
        protected IState State;
        protected IServiceLocator Locator;
        protected ObjectContext Context;
        EFUnitOfWorkFactory _unitOfWorkFactory;

        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
            _unitOfWorkFactory = new EFUnitOfWorkFactory();
            _unitOfWorkFactory.RegisterObjectContextProvider(() => new CodeOnlyContext("SandboxCodeOnly").Context);

            Locator = MockRepository.GenerateStub<IServiceLocator>();
            Locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(_unitOfWorkFactory);
            Locator.Stub(x => x.GetInstance<IState>()).Do(new Func<IState>(() => State));
            ServiceLocator.SetLocatorProvider(() => Locator);
        }

        [SetUp]
        public virtual void TestSetup()
        {
            State = new FakeState();
            Context = new CodeOnlyContext("SandboxCodeOnly").Context;
        }

        [TearDown]
        public void TestTeardown()
        {
            Context.ExecuteStoreCommand("DELETE OrderItems");
            Context.ExecuteStoreCommand("DELETE Products");
            Context.ExecuteStoreCommand("DELETE Orders");
            Context.ExecuteStoreCommand("DELETE Customers");
            Context.Dispose();
        }
    }
}