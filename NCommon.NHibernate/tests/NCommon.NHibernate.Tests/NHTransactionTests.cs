using System.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.NHibernate.Tests
{
    [TestFixture]
    public class NHTransactionTests
    {
        [Test]
        public void Commit_commits_all_NHibernate_transactions_handled_by_NHTransaction()
        {
            var tx = new NHTransaction(IsolationLevel.ReadCommitted);
            var nhTx1 = MockRepository.GenerateMock<global::NHibernate.ITransaction>();
            var nhTx2 = MockRepository.GenerateMock<global::NHibernate.ITransaction>();

            tx.RegisterTransaction(nhTx1);
            tx.RegisterTransaction(nhTx2);
            tx.Commit();

            nhTx1.AssertWasCalled(x => x.Commit());
            nhTx2.AssertWasCalled(x => x.Commit());
        }

        [Test]
        public void Rollback_rollsback_all_NHibernate_transactions_handled_by_NHTransaction()
        {
            var tx = new NHTransaction(IsolationLevel.ReadCommitted);
            var nhTx1 = MockRepository.GenerateMock<global::NHibernate.ITransaction>();
            var nhTx2 = MockRepository.GenerateMock<global::NHibernate.ITransaction>();

            tx.RegisterTransaction(nhTx1);
            tx.RegisterTransaction(nhTx2);
            tx.Rollback();

            nhTx1.AssertWasCalled(x => x.Rollback());
            nhTx2.AssertWasCalled(x => x.Rollback());
        }

        [Test]
        public void Dispose_calls_dispose_on_all_NHibernate_transactions_handled_by_NHTransaction()
        {
            var nhTx1 = MockRepository.GenerateMock<global::NHibernate.ITransaction>();
            var nhTx2 = MockRepository.GenerateMock<global::NHibernate.ITransaction>();
            using (var tx = new NHTransaction(IsolationLevel.ReadCommitted, nhTx1, nhTx2))
            {
            }
            nhTx1.AssertWasCalled(x => x.Dispose());
            nhTx2.AssertWasCalled(x => x.Dispose());
        }

        [Test]
        public void Commit_Raises_TransactionComitted_Event()
        {
            var tx = MockRepository.GenerateMock<global::NHibernate.ITransaction>();

            var commitCalled = false;
            var rollbackCalled = false;
            var transaction = new NHTransaction(IsolationLevel.Serializable);
            transaction.RegisterTransaction(tx);
            transaction.TransactionCommitted += delegate { commitCalled = true; };
            transaction.TransactionRolledback += delegate { rollbackCalled = true; };

            transaction.Commit();

            tx.AssertWasCalled(x => x.Commit());
            Assert.That(commitCalled);
            Assert.That(!rollbackCalled);
        }

        [Test]
        public void Rollback_Raises_RollbackComitted_Event()
        {
            var tx = MockRepository.GenerateMock<global::NHibernate.ITransaction>();

            var commitCalled = false;
            var rollbackCalled = false;
            var transaction = new NHTransaction(IsolationLevel.Serializable);
            transaction.RegisterTransaction(tx);
            transaction.TransactionCommitted += delegate { commitCalled = true; };
            transaction.TransactionRolledback += delegate { rollbackCalled = true; };

            transaction.Rollback();

            tx.AssertWasCalled(x => x.Rollback());
            Assert.That(!commitCalled);
            Assert.That(rollbackCalled);
        }
    }
}