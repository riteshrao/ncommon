using System;
using System.Transactions;
using NCommon.Data;
using NCommon.Data.Impl;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Tests.Data
{
    [TestFixture]
    public class UnitOfWorkTransactionTests
    {
        [Test]
        public void ctor_throws_ArgumentNullException_when_unitOfWork_parameter_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new UnitOfWorkTransaction(null, null));
        }

        [Test]
        public void ctor_throws_ArgumentNullException_when_transaction_parameter_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new UnitOfWorkTransaction(MockRepository.GenerateStub<IUnitOfWork>(), null));
        }

        [Test]
        public void EnlistScope_throws_ArgumentNullException_when_scope_parameter_is_null()
        {
            using (var transaction = new TransactionScope())
            {
                var uowTx = new UnitOfWorkTransaction(MockRepository.GenerateStub<IUnitOfWork>(), transaction);
                Assert.Throws<ArgumentNullException>(() => uowTx.EnlistScope(null));
            }
        }

        [Test]
        public void when_scope_is_comitting_unit_of_work_and_transaction_is_comitted()
        {
            using (var tx = new TransactionScope())
            {
                Assert.That(Transaction.Current, Is.Not.Null);
                Assert.That(Transaction.Current.TransactionInformation.Status, Is.EqualTo(TransactionStatus.Active));

                var uow = MockRepository.GenerateMock<IUnitOfWork>();
                var uowScope = MockRepository.GenerateStub<IUnitOfWorkScope>();
                var uowTx = new UnitOfWorkTransaction(uow, tx);
                uowTx.EnlistScope(uowScope);
                uowScope.Raise(x => x.ScopeComitting += null, uowScope);

                uow.AssertWasCalled(x => x.Flush());
                Assert.That(Transaction.Current, Is.Null);
            }
        }

        [Test]
        public void when_scope_is_comitted_unit_of_work_and_transaction_are_not_comitted_when_additional_scopes_are_attached()
        {
            using (var tx = new TransactionScope())
            {
                Assert.That(Transaction.Current, Is.Not.Null);
                Assert.That(Transaction.Current.TransactionInformation.Status, Is.EqualTo(TransactionStatus.Active));

                var uow = MockRepository.GenerateMock<IUnitOfWork>();
                var uowScope1 = MockRepository.GenerateStub<IUnitOfWorkScope>();
                var uowScope2 = MockRepository.GenerateStub<IUnitOfWorkScope>();
                var uowTx = new UnitOfWorkTransaction(uow, tx);

                uowScope2.Raise(x => x.ScopeComitting += null, uowScope2);

                uow.AssertWasNotCalled(x => x.Flush());
                Assert.That(Transaction.Current, Is.Not.Null);
                Assert.That(Transaction.Current.TransactionInformation.Status, Is.EqualTo(TransactionStatus.Active));
            }
        }

        [Test]
        public void when_scope_is_rolledback_unit_of_work_and_transaction_is_disposed()
        {
            using (var tx = new TransactionScope())
            {
                Assert.That(Transaction.Current, Is.Not.Null);
                Assert.That(Transaction.Current.TransactionInformation.Status, Is.EqualTo(TransactionStatus.Active));

                var uow = MockRepository.GenerateMock<IUnitOfWork>();
                var uowScope1 = MockRepository.GenerateStub<IUnitOfWorkScope>();
                var uowTx = new UnitOfWorkTransaction(uow, tx);
                uowTx.EnlistScope(uowScope1);
                uowScope1.Raise(x => x.ScopeRollingback += null, uowScope1);

                uow.AssertWasNotCalled(x => x.Flush());
                uow.AssertWasCalled(x => x.Dispose());
                Assert.That(Transaction.Current, Is.Null);
            }
        }

        [Test]
        public void when_scope_is_rolledback_unit_of_work_and_transaction_is_disposed_even_when_other_scopes_are_attached()
        {
            using (var tx = new TransactionScope())
            {
                Assert.That(Transaction.Current, Is.Not.Null);
                Assert.That(Transaction.Current.TransactionInformation.Status, Is.EqualTo(TransactionStatus.Active));

                var uow = MockRepository.GenerateMock<IUnitOfWork>();
                var uowScope1 = MockRepository.GenerateStub<IUnitOfWorkScope>();
                var uowScope2 = MockRepository.GenerateStub<IUnitOfWorkScope>();
                var uowTx = new UnitOfWorkTransaction(uow, tx);
                uowTx.EnlistScope(uowScope1);
                uowTx.EnlistScope(uowScope2);
                uowScope1.Raise(x => x.ScopeRollingback += null, uowScope2);

                uow.AssertWasNotCalled(x => x.Flush());
                uow.AssertWasCalled(x => x.Dispose());
                Assert.That(Transaction.Current, Is.Null);
            }
        }

        [Test]
        public void disposing_transaction_raises_TransactionDisposing_event()
        {
            var disposed = false;
            using (var tx = new UnitOfWorkTransaction(MockRepository.GenerateStub<IUnitOfWork>(),new TransactionScope()))
            {
                tx.TransactionDisposing += (x) => disposed = true;
            }
            Assert.That(disposed);
        }
    }
}