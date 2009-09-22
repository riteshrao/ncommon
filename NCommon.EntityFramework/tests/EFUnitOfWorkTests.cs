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

namespace NCommon.Data.EntityFramework.Tests
{
    [TestFixture]
    public class EFUnitOfWorkTests
    {

        [Test]
        public void Ctor_Throws_ArgumentNullException_When_DataContext_Parameter_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new EFUnitOfWork(null));
        }

        [Test]
        public void IsInTransaction_Should_Return_False_When_No_Transaction_Exists()
        {
            var mockDataContext = MockRepository.GenerateStub<IEFSession>();
            var unitOfWork = new EFUnitOfWork(mockDataContext);

            Assert.That(!unitOfWork.IsInTransaction);
        }

        [Test]
        public void Begin_Transaction_Should_Start_A_New_Transaction_With_Default_IsolationLevel()
        {
            var mockDataContext = MockRepository.GenerateMock<IEFSession>();
            var mockConnection = MockRepository.GenerateMock<IDbConnection>();
            var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

            mockDataContext.Stub(x => x.Connection).Return(mockConnection);
            mockConnection.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).Return(mockTransaction);

            var unitOfWork = new EFUnitOfWork(mockDataContext);
            Assert.That(!unitOfWork.IsInTransaction);
            unitOfWork.BeginTransaction();

            Assert.That(unitOfWork.IsInTransaction);

            //mockDataContext.VerifyAllExpectations();
            mockConnection.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();
        }

        [Test]
        public void BeginTransaction_Should_Start_A_New_Transaction_With_Specified_IsolatinLevel()
        {
            var isoLevel = IsolationLevel.Snapshot;
            var mockDataContext = MockRepository.GenerateMock<IEFSession>();
            var mockConnection = MockRepository.GenerateMock<IDbConnection>();
            var mockTransaction = MockRepository.GenerateStub<IDbTransaction>();

            mockDataContext.Stub(x => x.Connection).Return(mockConnection);
            mockConnection.Expect(x => x.BeginTransaction(isoLevel)).Return(mockTransaction);

            var unitOfWork = new EFUnitOfWork(mockDataContext);
            Assert.That(!unitOfWork.IsInTransaction);
            unitOfWork.BeginTransaction(IsolationLevel.Snapshot);
            Assert.That(unitOfWork.IsInTransaction);

            mockDataContext.VerifyAllExpectations();
            mockConnection.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();
        }

        [Test]
        public void BeginTransaction_Throws_InvalidOperationException_When_Transaction_Already_Running()
        {
            var mockDataContext = MockRepository.GenerateMock<IEFSession>();
            var mockConnection = MockRepository.GenerateMock<IDbConnection>();
            var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

            mockDataContext.Stub(x => x.Connection).Return(mockConnection);
            mockConnection.Expect(x => x.BeginTransaction(IsolationLevel.Unspecified))
                            .IgnoreArguments().Return(mockTransaction);

            var unitOfWork = new EFUnitOfWork(mockDataContext);
            unitOfWork.BeginTransaction();

            Assert.That(unitOfWork.IsInTransaction);
            Assert.Throws<InvalidOperationException>(() => unitOfWork.BeginTransaction());

            mockDataContext.VerifyAllExpectations();
            mockConnection.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();
        }

        [Test]
        public void Flush_Calls_Underlying_DataContext_SubmitChanges()
        {
            var mockDataContext = MockRepository.GenerateMock<IEFSession>();
            mockDataContext.Expect(x => x.SaveChanges());

            var unitOfWork = new EFUnitOfWork(mockDataContext);
            unitOfWork.Flush();

            mockDataContext.VerifyAllExpectations();
        }

        [Test]
        public void TransactionalFlush_Starts_A_Transaction_With_Default_Isolation_And_Commits_When_Flush_Succeeds()
        {
            var mockDataContext = MockRepository.GenerateMock<IEFSession>();
            var mockConnection = MockRepository.GenerateMock<IDbConnection>();
            var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

            mockDataContext.Stub(x => x.Connection).Return(mockConnection);
            mockConnection.Expect(x => x.BeginTransaction(IsolationLevel.Unspecified))
                          .IgnoreArguments()
                          .Return(mockTransaction);

            mockDataContext.Expect(x => x.SaveChanges());
            mockConnection.Expect(x => x.State).Return(ConnectionState.Closed); //First call should be closed
            mockConnection.Expect(x => x.Open());
            mockTransaction.Expect(x => x.Commit());
            mockConnection.Expect(x => x.State).Return(ConnectionState.Open); //Second call should be open
            mockConnection.Expect(x => x.Close());
            

            var unitOfWork = new EFUnitOfWork(mockDataContext);
            unitOfWork.TransactionalFlush();

            mockDataContext.VerifyAllExpectations();
            mockConnection.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();
        }

        [Test]
        public void TransactionalFlush_Starts_A_Transaction_With_Specified_IsolationLevel_And_Commits_When_Flush_Succeeds()
        {
            var mockDataContext = MockRepository.GenerateMock<IEFSession>();
            var mockConnection = MockRepository.GenerateMock<IDbConnection>();
            var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

            mockDataContext.Stub(x => x.Connection).Return(mockConnection);
            mockConnection.Expect(x => x.BeginTransaction(IsolationLevel.Unspecified))
                          .IgnoreArguments()
                          .Return(mockTransaction);
            mockDataContext.Expect(x => x.SaveChanges());
            mockTransaction.Expect(x => x.Commit());

            var unitOfWork = new EFUnitOfWork(mockDataContext);
            unitOfWork.TransactionalFlush(IsolationLevel.ReadUncommitted);

            mockDataContext.VerifyAllExpectations();
            mockConnection.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();
        }

        [Test]
        public void TransactionalFlush_Rollsback_Transaction_When_Flush_Throws_Exception()
        {
            var mockDataContext = MockRepository.GenerateMock<IEFSession>();
            var mockConnection = MockRepository.GenerateMock<IDbConnection>();
            var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

            mockDataContext.Stub(x => x.Connection).Return(mockConnection);
            mockConnection.Expect(x => x.BeginTransaction(IsolationLevel.Unspecified))
                          .IgnoreArguments()
                          .Return(mockTransaction);
            mockDataContext.Expect(x => x.SaveChanges()).Throw(new Exception());
            mockTransaction.Expect(x => x.Rollback());

            var unitOfWork = new EFUnitOfWork(mockDataContext);
            Assert.Throws<Exception>(unitOfWork.TransactionalFlush);

            mockDataContext.VerifyAllExpectations();
            mockConnection.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();
        }

        [Test]
        public void TransactionalFlush_Uses_Existing_Transaction_When_Transactional_AlreadyRunning()
        {
            var mockDataContext = MockRepository.GenerateMock<IEFSession>();
            var mockConnection = MockRepository.GenerateMock<IDbConnection>();
            var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

            mockDataContext.Expect(x => x.Connection).Repeat.Any().Return(mockConnection);
            mockConnection.Stub(x => x.BeginTransaction(IsolationLevel.Unspecified))
                          .IgnoreArguments()
                          .Repeat.Once()
                          .Return(mockTransaction);


            var unitOfWork = new EFUnitOfWork(mockDataContext);

            unitOfWork.BeginTransaction();
            unitOfWork.TransactionalFlush();

            mockDataContext.VerifyAllExpectations();
            mockConnection.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();
        }

        [Test]
        public void Comitting_Transaction_Releases_Transaction_From_UnitOfWork()
        {
            var mockDataContext = MockRepository.GenerateMock<IEFSession>();
            var mockConnection = MockRepository.GenerateMock<IDbConnection>();
            var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

            mockDataContext.Stub(x => x.Connection).Return(mockConnection);
            mockConnection.Expect(x => x.BeginTransaction(IsolationLevel.Unspecified))
                          .IgnoreArguments()
                          .Return(mockTransaction);
            mockTransaction.Expect(x => x.Commit());

            var unitOfWork = new EFUnitOfWork(mockDataContext);
            var transaction = unitOfWork.BeginTransaction();

            Assert.That(unitOfWork.IsInTransaction);
            transaction.Commit();
            Assert.That(!unitOfWork.IsInTransaction);

            mockDataContext.VerifyAllExpectations();
            mockConnection.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();
        }

        [Test]
        public void Rollback_Transaction_Releases_Transaction_From_UnitOfWork()
        {
            var mockDataContext = MockRepository.GenerateMock<IEFSession>();
            var mockConnection = MockRepository.GenerateMock<IDbConnection>();
            var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

            mockDataContext.Stub(x => x.Connection).Return(mockConnection);
            mockConnection.Expect(x => x.BeginTransaction(IsolationLevel.Unspecified))
                          .IgnoreArguments()
                          .Return(mockTransaction);
            mockTransaction.Expect(x => x.Rollback());

            var unitOfWork = new EFUnitOfWork(mockDataContext);
            var transaction = unitOfWork.BeginTransaction();

            Assert.That(unitOfWork.IsInTransaction);
            transaction.Rollback();
            Assert.That(!unitOfWork.IsInTransaction);

            mockDataContext.VerifyAllExpectations();
            mockConnection.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();
        }

        [Test]
        public void Dispose_UnitOfWork_Disposed_Underlying_Transaction_And_Session()
        {
            var mockDataContext = MockRepository.GenerateMock<IEFSession>();
            var mockConnection = MockRepository.GenerateMock<IDbConnection>();
            var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

            mockDataContext.Stub(x => x.Connection).Return(mockConnection);
            mockConnection.Expect(x => x.BeginTransaction(IsolationLevel.Unspecified))
                          .IgnoreArguments()
                          .Return(mockTransaction);

            mockTransaction.Expect(x => x.Dispose());
            mockDataContext.Expect(x => x.Dispose());

            using (var unitOfWork = new EFUnitOfWork(mockDataContext))
            {
                unitOfWork.BeginTransaction();
            }

            mockDataContext.VerifyAllExpectations();
            mockConnection.VerifyAllExpectations();
            mockTransaction.VerifyAllExpectations();
        }
    }
}
