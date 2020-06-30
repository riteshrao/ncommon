using System;
using CommonServiceLocator;
using NCommon.DataServices.Transactions;

using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Tests.Data
{
    [TestFixture]
    public class TransactionManagerTests
    {
        [Test]
        public void CurrentUnitOfWork_returns_null_with_no_enlisted_scope()
        {
            using (var txManager = new UnitOfWorkTransactionManager())
            {
                Assert.That(txManager.CurrentUnitOfWork, Is.Null);    
            }
        }

        [Test]
        public void CurrentTransaction_returns_null_with_no_enlisted_scope()
        {
            using (var txManager = new UnitOfWorkTransactionManager())
            {
                Assert.That(txManager.CurrentTransaction, Is.Null);
            }
        }

        [Test]
        public void new_transaction_is_started_when_scope_is_enlisted()
        {
            var uowFactory = MockRepository.GenerateStub<IUnitOfWorkFactory>();
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            uowFactory.Stub(x => x.Create())
                .Do((Func<IUnitOfWork>) (() => MockRepository.GenerateStub<IUnitOfWork>()));
            locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(uowFactory);
            ServiceLocator.SetLocatorProvider(() => locator);
            using (var txManager = new UnitOfWorkTransactionManager())
            {
                txManager.EnlistScope(MockRepository.GenerateStub<IUnitOfWorkScope>(), TransactionMode.Default);
                Assert.That(txManager.CurrentUnitOfWork, Is.Not.Null);
                Assert.That(txManager.CurrentTransaction, Is.Not.Null);
            }
        }

        [Test]
        public void new_transaction_is_not_started_when_second_scope_is_enlisted()
        {
            var uowFactory = MockRepository.GenerateStub<IUnitOfWorkFactory>();
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            uowFactory.Stub(x => x.Create())
                .Do((Func<IUnitOfWork>)(() => MockRepository.GenerateStub<IUnitOfWork>()));
            locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(uowFactory);
            ServiceLocator.SetLocatorProvider(() => locator);
            using (var txManager = new UnitOfWorkTransactionManager())
            {
                txManager.EnlistScope(MockRepository.GenerateStub<IUnitOfWorkScope>(), TransactionMode.Default);
                var currentUOW = txManager.CurrentUnitOfWork;
                var currentTx = txManager.CurrentTransaction;

                txManager.EnlistScope(MockRepository.GenerateStub<IUnitOfWorkScope>(), TransactionMode.Default);
                Assert.That(currentUOW, Is.SameAs(txManager.CurrentUnitOfWork));
                Assert.That(currentTx, Is.SameAs(txManager.CurrentTransaction));
            }
        }

        [Test]
        public void new_transaction_is_started_when_second_scope_is_enlisted_with_TransactionMode_New()
        {
            var uowFactory = MockRepository.GenerateStub<IUnitOfWorkFactory>();
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            uowFactory.Stub(x => x.Create())
                .Do((Func<IUnitOfWork>)(() => MockRepository.GenerateStub<IUnitOfWork>()));
            locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(uowFactory);
            ServiceLocator.SetLocatorProvider(() => locator);
            using (var txManager = new UnitOfWorkTransactionManager())
            {
                txManager.EnlistScope(MockRepository.GenerateStub<IUnitOfWorkScope>(), TransactionMode.Default);
                var currentUOW = txManager.CurrentUnitOfWork;
                var currentTx = txManager.CurrentTransaction;

                txManager.EnlistScope(MockRepository.GenerateStub<IUnitOfWorkScope>(), TransactionMode.New);
                Assert.That(currentUOW, Is.Not.SameAs(txManager.CurrentUnitOfWork));
                Assert.That(currentTx, Is.Not.SameAs(txManager.CurrentTransaction));
            }
        }

        [Test]
        public void new_transaction_is_started_when_second_scope_is_enlisted_with_TransactionMode_Supress()
        {
            var uowFactory = MockRepository.GenerateStrictMock<IUnitOfWorkFactory>();
            var locator = MockRepository.GenerateStrictMock<IServiceLocator>();
            uowFactory.Stub(x => x.Create())
                .Do((Func<IUnitOfWork>) (() => MockRepository.GenerateStub<IUnitOfWork>()));
            locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(uowFactory);
            ServiceLocator.SetLocatorProvider(() => locator);
            using (var txManager = new UnitOfWorkTransactionManager())
            {
                txManager.EnlistScope(MockRepository.GenerateStub<IUnitOfWorkScope>(), TransactionMode.Default);
                var parentUOW = txManager.CurrentUnitOfWork;
                var parentTx = txManager.CurrentTransaction;
                
                txManager.EnlistScope(MockRepository.GenerateStub<IUnitOfWorkScope>(), TransactionMode.Supress);
                Assert.That(parentUOW, Is.Not.SameAs(txManager.CurrentUnitOfWork));
                Assert.That(parentTx, Is.Not.SameAs(txManager.CurrentTransaction));
            }
        }

        [Test]
        public void CurrentTransaction_returns_null_when_scope_is_comitted()
        {
            var uowFactory = MockRepository.GenerateStub<IUnitOfWorkFactory>();
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            uowFactory.Stub(x => x.Create())
                .Do((Func<IUnitOfWork>)(() => MockRepository.GenerateStub<IUnitOfWork>()));

            locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(uowFactory);
            ServiceLocator.SetLocatorProvider(() => locator);

            using (var txManager = new UnitOfWorkTransactionManager())
            {
                var scope = MockRepository.GenerateStub<IUnitOfWorkScope>();
                txManager.EnlistScope(scope, TransactionMode.Default);

                Assert.That(txManager.CurrentUnitOfWork, Is.Not.Null);
                Assert.That(txManager.CurrentTransaction, Is.Not.Null);
                scope.Raise(x => x.ScopeComitting += null, scope);

                Assert.That(txManager.CurrentTransaction, Is.Null);
                Assert.That(txManager.CurrentUnitOfWork, Is.Null);
            }
        }

        [Test]
        public void CurrentTransaction_returns_null_when_scope_is_rolledback()
        {
            var uowFactory = MockRepository.GenerateStub<IUnitOfWorkFactory>();
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            uowFactory.Stub(x => x.Create())
                .Do((Func<IUnitOfWork>)(() => MockRepository.GenerateStub<IUnitOfWork>()));

            locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(uowFactory);
            ServiceLocator.SetLocatorProvider(() => locator);

            using (var txManager = new UnitOfWorkTransactionManager())
            {
                var scope = MockRepository.GenerateStub<IUnitOfWorkScope>();
                txManager.EnlistScope(scope, TransactionMode.Default);

                Assert.That(txManager.CurrentUnitOfWork, Is.Not.Null);
                Assert.That(txManager.CurrentTransaction, Is.Not.Null);
                scope.Raise(x => x.ScopeRollingback += null, scope);

                Assert.That(txManager.CurrentTransaction, Is.Null);
                Assert.That(txManager.CurrentUnitOfWork, Is.Null);
            }
        }
    }
}