
using NCommon.DataServices.Transactions;
using NCommon.StateStorage;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Tests.Data
{
    [TestFixture]
    public class UnitOfWorkManagerTests
    {
        [Test]
        public void CurrentTransactionManager_returns_transaction_manager_using_custom_provider()
        {
            UnitOfWorkManager.SetTransactionManagerProvider(() => MockRepository.GenerateStub<IUnitOfWorkTransactionManager>());
            Assert.That(UnitOfWorkManager.CurrentTransactionManager, Is.Not.Null);
        }

        [Test]
        public void CurrentUnitOfWork_returns_unit_of_work_from_current_transaction_manager()
        {
            var txManager = MockRepository.GenerateStub<IUnitOfWorkTransactionManager>();
            txManager.Stub(x => x.CurrentUnitOfWork).Return(MockRepository.GenerateStub<IUnitOfWork>());
            UnitOfWorkManager.SetTransactionManagerProvider(() => txManager);

            Assert.That(UnitOfWorkManager.CurrentUnitOfWork, Is.Not.Null);
        }

        [Test]
        public void CurrentTransactionManager_returns_transaction_manager_from_local_state()
        {
            var state = MockRepository.GenerateStub<IState>();
            var txManager = MockRepository.GenerateStub<IUnitOfWorkTransactionManager>();
            state.Stub(x => x.Local).Return(MockRepository.GenerateStub<ILocalState>());
            state.Local
                .Stub(x => x.Get<IUnitOfWorkTransactionManager>(Arg<string>.Is.Anything))
                .Return(txManager);

            var locator = MockRepository.GenerateStub<IServiceLocator>();
            locator.Stub(x => x.GetInstance<IState>()).Return(state);
            ServiceLocator.SetLocatorProvider(() => locator);
            Assert.That(UnitOfWorkManager.CurrentTransactionManager, Is.SameAs(txManager));
        }
    }
}