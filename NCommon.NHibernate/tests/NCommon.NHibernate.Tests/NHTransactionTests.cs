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

            tx.RegisterNHTransaction(nhTx1);
            tx.RegisterNHTransaction(nhTx2);
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

            tx.RegisterNHTransaction(nhTx1);
            tx.RegisterNHTransaction(nhTx2);
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
    }
}