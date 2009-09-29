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
        public void Ctor_Throws_ArgumentNullException_When_ITransation_Parameter_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new LinqToSqlTransaction(null));
        }

        [Test]
        public void Commit_Calls_Commit_On_Underlying_ITransaction()
        {
            var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();
            mockTransaction.Expect(x => x.Commit()).IgnoreArguments();

            var transaction = new LinqToSqlTransaction(mockTransaction);
            transaction.Commit();

            mockTransaction.VerifyAllExpectations();
        }

        [Test]
        public void Rollback_Calls_Rollback_On_Underlying_ITransaction()
        {
            var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();
            mockTransaction.Expect(x => x.Rollback()).IgnoreArguments();

            var transaction = new LinqToSqlTransaction(mockTransaction);
            transaction.Rollback();

            mockTransaction.VerifyAllExpectations();
        }

        [Test]
        public void Commit_Raises_TransactionComitted_Event()
        {
            var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();
            mockTransaction.Expect(x => x.Commit());

            bool commitCalled = false;
            bool rollbackCalled = false;
            var transaction = new LinqToSqlTransaction(mockTransaction);
            transaction.TransactionCommitted += delegate { commitCalled = true; };
            transaction.TransactionRolledback += delegate { rollbackCalled = true; };

            transaction.Commit();

            mockTransaction.VerifyAllExpectations();
            Assert.That(commitCalled);
            Assert.That(!rollbackCalled);
        }

        [Test]
        public void Rollback_Raises_RollbackComitted_Event()
        {
            var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();
            mockTransaction.Expect(x => x.Rollback());

            bool commitCalled = false;
            bool rollbackCalled = false;
            var transaction = new LinqToSqlTransaction(mockTransaction);
            transaction.TransactionCommitted += delegate { commitCalled = true; };
            transaction.TransactionRolledback += delegate { rollbackCalled = true; };

            transaction.Rollback();

            mockTransaction.VerifyAllExpectations();
            Assert.That(!commitCalled);
            Assert.That(rollbackCalled);
        }
    }
}
