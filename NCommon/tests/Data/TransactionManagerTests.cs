using System;
using Microsoft.Practices.ServiceLocation;
using NCommon.Data;
using NCommon.Data.Impl;
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
            using (var txManager = new TransactionManager())
            {
                Assert.That(txManager.CurrentUnitOfWork, Is.Null);    
            }
        }

        [Test]
        public void CurrentTransaction_returns_null_with_no_enlisted_scope()
        {
            using (var txManager = new TransactionManager())
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
            using (var txManager = new TransactionManager())
            {
                txManager.EnlistScope(MockRepository.GenerateStub<IUnitOfWorkScope>(), false);
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
            using (var txManager = new TransactionManager())
            {
                txManager.EnlistScope(MockRepository.GenerateStub<IUnitOfWorkScope>(), false);
                var currentUOW = txManager.CurrentUnitOfWork;
                var currentTx = txManager.CurrentTransaction;

                txManager.EnlistScope(MockRepository.GenerateStub<IUnitOfWorkScope>(), false);
                Assert.That(currentUOW, Is.SameAs(txManager.CurrentUnitOfWork));
                Assert.That(currentTx, Is.SameAs(txManager.CurrentTransaction));
            }
        }

        [Test]
        public void new_transaction_is_started_when_second_scope_is_enlisted_with_newTransaction_as_true()
        {
            var uowFactory = MockRepository.GenerateStub<IUnitOfWorkFactory>();
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            uowFactory.Stub(x => x.Create())
                .Do((Func<IUnitOfWork>)(() => MockRepository.GenerateStub<IUnitOfWork>()));
            locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(uowFactory);
            ServiceLocator.SetLocatorProvider(() => locator);
            using (var txManager = new TransactionManager())
            {
                txManager.EnlistScope(MockRepository.GenerateStub<IUnitOfWorkScope>(), false);
                var currentUOW = txManager.CurrentUnitOfWork;
                var currentTx = txManager.CurrentTransaction;

                txManager.EnlistScope(MockRepository.GenerateStub<IUnitOfWorkScope>(), true);
                Assert.That(currentUOW, Is.Not.SameAs(txManager.CurrentUnitOfWork));
                Assert.That(currentTx, Is.Not.SameAs(txManager.CurrentTransaction));
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

            using (var txManager = new TransactionManager())
            {
                var scope = MockRepository.GenerateStub<IUnitOfWorkScope>();
                txManager.EnlistScope(scope, false);

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

            using (var txManager = new TransactionManager())
            {
                var scope = MockRepository.GenerateStub<IUnitOfWorkScope>();
                txManager.EnlistScope(scope, false);

                Assert.That(txManager.CurrentUnitOfWork, Is.Not.Null);
                Assert.That(txManager.CurrentTransaction, Is.Not.Null);
                scope.Raise(x => x.ScopeRollingback += null, scope);

                Assert.That(txManager.CurrentTransaction, Is.Null);
                Assert.That(txManager.CurrentUnitOfWork, Is.Null);
            }
        }
    }
}