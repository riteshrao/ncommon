#region license
//Copyright 2008 Ritesh Rao 

//Licensed under the Apache License, Version 2.0 (the "License"); 
//you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 

//http://www.apache.org/licenses/LICENSE-2.0 

//Unless required by applicable law or agreed to in writing, software 
//distributed under the License is distributed on an "AS IS" BASIS, 
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and 
//limitations under the License. 
#endregion

using System;
using System.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.LinqToSql.Tests
{
    /// <summary>
    /// Tests the <see cref="LinqToSqlTransaction"/> class.
    /// </summary>
    [TestFixture]
    public class LinqToSqlTransactionTests
    {
        [Test]
        public void Commit_commits_all_IDbTransaction_transactions_handled_by_LinqToSqlTransaction()
        {
            var tx = new LinqToSqlTransaction(IsolationLevel.ReadCommitted);
            var nhTx1 = MockRepository.GenerateMock<IDbTransaction>();
            var nhTx2 = MockRepository.GenerateMock<IDbTransaction>();

            tx.RegisterTransaction(nhTx1);
            tx.RegisterTransaction(nhTx2);
            tx.Commit();

            nhTx1.AssertWasCalled(x => x.Commit());
            nhTx2.AssertWasCalled(x => x.Commit());
        }

        [Test]
        public void Rollback_rollsback_all_NHibernate_transactions_handled_by_NHTransaction()
        {
            var tx = new LinqToSqlTransaction(IsolationLevel.ReadCommitted);
            var nhTx1 = MockRepository.GenerateMock<IDbTransaction>();
            var nhTx2 = MockRepository.GenerateMock<IDbTransaction>();

            tx.RegisterTransaction(nhTx1);
            tx.RegisterTransaction(nhTx2);
            tx.Rollback();

            nhTx1.AssertWasCalled(x => x.Rollback());
            nhTx2.AssertWasCalled(x => x.Rollback());
        }

        [Test]
        public void Dispose_calls_dispose_on_all_NHibernate_transactions_handled_by_NHTransaction()
        {
            var nhTx1 = MockRepository.GenerateMock<IDbTransaction>();
            var nhTx2 = MockRepository.GenerateMock<IDbTransaction>();
            using (var tx = new LinqToSqlTransaction(IsolationLevel.ReadCommitted, nhTx1, nhTx2))
            {
            }
            nhTx1.AssertWasCalled(x => x.Dispose());
            nhTx2.AssertWasCalled(x => x.Dispose());
        }

        [Test]
        public void Commit_Raises_TransactionComitted_Event()
        {
            var tx = MockRepository.GenerateMock<IDbTransaction>();

            var commitCalled = false;
            var rollbackCalled = false;
            var transaction = new LinqToSqlTransaction(IsolationLevel.Serializable);
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
            var tx = MockRepository.GenerateMock<IDbTransaction>();

            var commitCalled = false;
            var rollbackCalled = false;
            var transaction = new LinqToSqlTransaction(IsolationLevel.Serializable);
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