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
        public void Ctor_Throws_ArgumentNullException_when_settings_parameter_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new EFUnitOfWork(null));
        }

        [Test]
        public void GetSession_returns_session_from_session_resolver()
        {
            var resolver = MockRepository.GenerateMock<IEFSessionResolver>();
            resolver.Expect(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Expect(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateStub<IEFSession>());

            var unitOfWork = new EFUnitOfWork(new EFUnitOfWorkSettings { SessionResolver = resolver });
            var session = unitOfWork.GetSession<string>();
            Assert.That(session, Is.Not.Null);
            resolver.VerifyAllExpectations();
        }

        [Test]
        public void GetSession_returns_same_session_instance_once_opened()
        {
            var resolver = MockRepository.GenerateMock<IEFSessionResolver>();
            resolver.Expect(x => x.GetSessionKeyFor<string>())
                .Return(Guid.NewGuid())
                .Repeat.Twice();

            resolver.Expect(x => x.OpenSessionFor<string>())
                .Return(MockRepository.GenerateStub<IEFSession>())
                .Repeat.Once();

            var unitOfWork = new EFUnitOfWork(new EFUnitOfWorkSettings { SessionResolver = resolver });
            var session1 = unitOfWork.GetSession<string>();
            var session2 = unitOfWork.GetSession<string>();
            Assert.That(session1, Is.Not.Null);
            Assert.That(session2, Is.Not.Null);
            Assert.That(session1, Is.SameAs(session2));
            resolver.VerifyAllExpectations();
        }

        [Test]
        public void GetSession_returns_seperate_session_instances_for_different_types()
        {
            var resolver = MockRepository.GenerateMock<IEFSessionResolver>();
            resolver.Expect(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Expect(x => x.GetSessionKeyFor<int>()).Return(Guid.NewGuid());
            resolver.Expect(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateStub<IEFSession>());
            resolver.Expect(x => x.OpenSessionFor<int>()).Return(MockRepository.GenerateStub<IEFSession>());

            var unitOfWork = new EFUnitOfWork(new EFUnitOfWorkSettings { SessionResolver = resolver });
            var session1 = unitOfWork.GetSession<string>();
            var session2 = unitOfWork.GetSession<int>();
            Assert.That(session1, Is.Not.Null);
            Assert.That(session2, Is.Not.Null);
            Assert.That(session1, Is.Not.SameAs(session2));
            resolver.VerifyAllExpectations();
        }

        [Test]
        public void GetSession_calls_BeginTransaction_on_session_being_opened_if_transaction_has_already_started()
        {
            var resolver = MockRepository.GenerateStub<IEFSessionResolver>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateStub<IEFSession>());
            resolver.OpenSessionFor<string>().Stub(x => x.Connection).Return(MockRepository.GenerateMock<IDbConnection>());
            resolver.OpenSessionFor<string>().Connection.Expect(x => x.State).Return(ConnectionState.Closed);
            resolver.OpenSessionFor<string>().Connection.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(MockRepository.GenerateStub<IDbTransaction>());

            var unitOfWork = new EFUnitOfWork(new EFUnitOfWorkSettings
            {
                DefaultIsolation = IsolationLevel.ReadCommitted,
                SessionResolver = resolver
            });

            unitOfWork.BeginTransaction();
            var session = unitOfWork.GetSession<string>();
            session.Connection.VerifyAllExpectations();
        }

        [Test]
        public void IsInTransaction_Should_Return_False_When_No_Transaction_Exists()
        {
            var unitOfWork = new EFUnitOfWork(new EFUnitOfWorkSettings());
            Assert.That(!unitOfWork.IsInTransaction);
        }

        [Test]
        public void Begin_Transaction_Should_Start_A_New_Transaction_With_Default_IsolationLevel()
        {
            var settings = new EFUnitOfWorkSettings { DefaultIsolation = IsolationLevel.Chaos };
            var unitOfWork = new EFUnitOfWork(settings);
            var transaction = unitOfWork.BeginTransaction();
            Assert.That(transaction.IsolationLevel, Is.EqualTo(settings.DefaultIsolation));
        }

        [Test]
        public void BeginTransaction_Should_Start_A_New_Transaction_With_Specified_IsolatinLevel()
        {
            var settings = new EFUnitOfWorkSettings { DefaultIsolation = IsolationLevel.Chaos };
            var unitOfWork = new EFUnitOfWork(settings);
            var transaction = unitOfWork.BeginTransaction(IsolationLevel.ReadCommitted);
            Assert.That(transaction.IsolationLevel, Is.Not.EqualTo(settings.DefaultIsolation));
            Assert.That(transaction.IsolationLevel, Is.EqualTo(IsolationLevel.ReadCommitted));

        }

        [Test]
        public void BeginTransaction_Throws_InvalidOperationException_When_Transaction_Already_Running()
        {
            var settings = new EFUnitOfWorkSettings();
            var unitOfWork = new EFUnitOfWork(settings);
            unitOfWork.BeginTransaction();
            Assert.Throws<InvalidOperationException>(() => unitOfWork.BeginTransaction());
        }

        [Test]
        public void BeginTransaction_calls_BeginTransaction_on_sessions_already_opened()
        {
            var resolver = MockRepository.GenerateStub<IEFSessionResolver>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<IEFSession>());
            resolver.OpenSessionFor<string>().Stub(x => x.Connection).Return(MockRepository.GenerateStub<IDbConnection>());
            resolver.OpenSessionFor<string>().Connection
                .Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(MockRepository.GenerateMock<IDbTransaction>());

            var unitOfWork = new EFUnitOfWork(new EFUnitOfWorkSettings
            {
                DefaultIsolation = IsolationLevel.ReadCommitted,
                SessionResolver = resolver
            });

            var session = unitOfWork.GetSession<string>();
            unitOfWork.BeginTransaction();
            session.VerifyAllExpectations();
        }

        [Test]
        public void Flush_Calls_Underlying_ISession_Flush()
        {
            var resolver = MockRepository.GenerateStub<IEFSessionResolver>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<IEFSession>());

            var unitOfWork = new EFUnitOfWork(new EFUnitOfWorkSettings { SessionResolver = resolver });
            var session = unitOfWork.GetSession<string>();
            unitOfWork.Flush();
            session.AssertWasCalled(x => x.SaveChanges());
        }

        [Test]
        public void TransactionalFlush_Starts_A_Transaction_With_Default_Isolation_And_Commits_When_Flush_Succeeds()
        {
            var resolver = MockRepository.GenerateStub<IEFSessionResolver>();
            var transaction = MockRepository.GenerateMock<IDbTransaction>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateStub<IEFSession>());
            resolver.OpenSessionFor<string>().Stub(x => x.Connection).Return(MockRepository.GenerateMock<IDbConnection>());
            resolver.OpenSessionFor<string>().Connection
                .Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(transaction);

            var unitOfWork = new EFUnitOfWork(new EFUnitOfWorkSettings
            {
                DefaultIsolation = IsolationLevel.ReadCommitted,
                SessionResolver = resolver
            });

            var session = unitOfWork.GetSession<string>();
            unitOfWork.TransactionalFlush();
            transaction.AssertWasCalled(x => x.Commit());
            session.VerifyAllExpectations();
        }

        [Test]
        public void TransactionalFlush_Starts_A_Transaction_With_Specified_IsolationLevel_And_Commits_When_Flush_Succeeds()
        {
            var resolver = MockRepository.GenerateStub<IEFSessionResolver>();
            var transaction = MockRepository.GenerateMock<IDbTransaction>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<IEFSession>());
            resolver.OpenSessionFor<string>().Stub(x => x.Connection).Return(MockRepository.GenerateMock<IDbConnection>());
            resolver.OpenSessionFor<string>().Connection
                .Expect(x => x.BeginTransaction(IsolationLevel.Serializable))
                .Return(transaction);

            var unitOfWork = new EFUnitOfWork(new EFUnitOfWorkSettings
            {
                DefaultIsolation = IsolationLevel.ReadCommitted,
                SessionResolver = resolver
            });

            var session = unitOfWork.GetSession<string>();
            unitOfWork.TransactionalFlush(IsolationLevel.Serializable);
            transaction.AssertWasCalled(x => x.Commit());
            session.VerifyAllExpectations();
        }

        [Test]
        public void TransactionalFlush_Rollsback_Transaction_When_TransactionalFlush_Throws_Exception()
        {
            var resolver = MockRepository.GenerateStub<IEFSessionResolver>();
            var transaction = MockRepository.GenerateMock<IDbTransaction>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<IEFSession>());
            resolver.OpenSessionFor<string>().Stub(x => x.Connection).Return(MockRepository.GenerateMock<IDbConnection>());
            resolver.OpenSessionFor<string>().Connection
                .Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(transaction);
            transaction.Expect(x => x.Commit()).Throw(new ApplicationException());

            var unitOfWork = new EFUnitOfWork(new EFUnitOfWorkSettings
            {
                DefaultIsolation = IsolationLevel.ReadCommitted,
                SessionResolver = resolver
            });

            var session = unitOfWork.GetSession<string>();
            Assert.Throws<ApplicationException>(unitOfWork.TransactionalFlush);
            transaction.AssertWasCalled(x => x.Rollback());
            transaction.VerifyAllExpectations();
            session.VerifyAllExpectations();
        }

        [Test]
        public void TransactionalFlush_Uses_Existing_Transaction_When_Transactional_AlreadyRunning()
        {
            var resolver = MockRepository.GenerateStub<IEFSessionResolver>();
            var transaction = MockRepository.GenerateMock<IDbTransaction>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<IEFSession>());
            resolver.OpenSessionFor<string>().Stub(x => x.Connection).Return(MockRepository.GenerateMock<IDbConnection>());
            resolver.OpenSessionFor<string>().Connection
                .Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(transaction)
                .Repeat.Once();

            var unitOfWork = new EFUnitOfWork(new EFUnitOfWorkSettings
            {
                DefaultIsolation = IsolationLevel.ReadCommitted,
                SessionResolver = resolver
            });

            var session = unitOfWork.GetSession<string>();
            unitOfWork.BeginTransaction();
            unitOfWork.TransactionalFlush();
            transaction.AssertWasCalled(x => x.Commit());
            session.VerifyAllExpectations();
        }

        [Test]
        public void Comitting_Transaction_Releases_Transaction_From_UnitOfWork()
        {
            var resolver = MockRepository.GenerateStub<IEFSessionResolver>();
            var transaction = MockRepository.GenerateMock<IDbTransaction>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<IEFSession>());
            resolver.OpenSessionFor<string>().Stub(x => x.Connection).Return(MockRepository.GenerateMock<IDbConnection>());
            resolver.OpenSessionFor<string>().Connection
                .Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(transaction);

            var unitOfWork = new EFUnitOfWork(new EFUnitOfWorkSettings
            {
                DefaultIsolation = IsolationLevel.ReadCommitted,
                SessionResolver = resolver
            });

            var session = unitOfWork.GetSession<string>();
            var tx = unitOfWork.BeginTransaction();
            tx.Commit();
            Assert.That(!unitOfWork.IsInTransaction);
            transaction.AssertWasCalled(x => x.Commit());
            transaction.AssertWasCalled(x => x.Dispose());
            session.VerifyAllExpectations();
        }

        [Test]
        public void Rollback_Transaction_Releases_Transaction_From_UnitOfWork()
        {
            var resolver = MockRepository.GenerateStub<IEFSessionResolver>();
            var transaction = MockRepository.GenerateMock<IDbTransaction>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<IEFSession>());
            resolver.OpenSessionFor<string>().Stub(x => x.Connection).Return(MockRepository.GenerateMock<IDbConnection>());
            resolver.OpenSessionFor<string>().Connection
                .Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(transaction);

            var unitOfWork = new EFUnitOfWork(new EFUnitOfWorkSettings
            {
                DefaultIsolation = IsolationLevel.ReadCommitted,
                SessionResolver = resolver
            });

            var session = unitOfWork.GetSession<string>();
            var tx = unitOfWork.BeginTransaction();
            tx.Rollback();
            Assert.That(!unitOfWork.IsInTransaction);
            transaction.AssertWasCalled(x => x.Rollback());
            transaction.AssertWasCalled(x => x.Dispose());
            session.VerifyAllExpectations();
        }

        [Test]
        public void disposing_EFUnitOfWork_disposes_off_open_transactions_and_open_sessions()
        {
            var resolver = MockRepository.GenerateStub<IEFSessionResolver>();
            var transaction = MockRepository.GenerateMock<IDbTransaction>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<IEFSession>());
            resolver.OpenSessionFor<string>().Stub(x => x.Connection).Return(MockRepository.GenerateMock<IDbConnection>());
            resolver.OpenSessionFor<string>().Connection
                .Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(transaction);

            resolver.Stub(x => x.GetSessionKeyFor<int>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<int>()).Return(MockRepository.GenerateMock<IEFSession>());
            resolver.OpenSessionFor<int>().Stub(x => x.Connection).Return(MockRepository.GenerateMock<IDbConnection>());
            resolver.OpenSessionFor<int>().Connection
                .Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(transaction);

            var unitOfWork = new EFUnitOfWork(new EFUnitOfWorkSettings
            {
                DefaultIsolation = IsolationLevel.ReadCommitted,
                SessionResolver = resolver
            });

            var session1 = unitOfWork.GetSession<string>();
            var session2 = unitOfWork.GetSession<int>();
            unitOfWork.BeginTransaction();
            unitOfWork.Dispose();
            Assert.That(!unitOfWork.IsInTransaction);
            transaction.AssertWasCalled(x => x.Dispose());
            session1.AssertWasCalled(x => x.Dispose());
            session2.AssertWasCalled(x => x.Dispose());
            session1.VerifyAllExpectations();
            session2.VerifyAllExpectations();
        }

        //[Test]
        //public void Ctor_Throws_ArgumentNullException_When_DataContext_Parameter_Is_Null()
        //{
        //    Assert.Throws<ArgumentNullException>(() => new EFUnitOfWork(null));
        //}

        //[Test]
        //public void IsInTransaction_Should_Return_False_When_No_Transaction_Exists()
        //{
        //    var mockDataContext = MockRepository.GenerateStub<IEFSession>();
        //    var unitOfWork = new EFUnitOfWork(mockDataContext);

        //    Assert.That(!unitOfWork.IsInTransaction);
        //}

        //[Test]
        //public void Begin_Transaction_Should_Start_A_New_Transaction_With_Default_IsolationLevel()
        //{
        //    var mockDataContext = MockRepository.GenerateMock<IEFSession>();
        //    var mockConnection = MockRepository.GenerateMock<IDbConnection>();
        //    var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

        //    mockDataContext.Stub(x => x.Connection).Return(mockConnection);
        //    mockConnection.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted)).Return(mockTransaction);

        //    var unitOfWork = new EFUnitOfWork(mockDataContext);
        //    Assert.That(!unitOfWork.IsInTransaction);
        //    unitOfWork.BeginTransaction();

        //    Assert.That(unitOfWork.IsInTransaction);

        //    //mockDataContext.VerifyAllExpectations();
        //    mockConnection.VerifyAllExpectations();
        //    mockTransaction.VerifyAllExpectations();
        //}

        //[Test]
        //public void BeginTransaction_Should_Start_A_New_Transaction_With_Specified_IsolatinLevel()
        //{
        //    var isoLevel = IsolationLevel.Snapshot;
        //    var mockDataContext = MockRepository.GenerateMock<IEFSession>();
        //    var mockConnection = MockRepository.GenerateMock<IDbConnection>();
        //    var mockTransaction = MockRepository.GenerateStub<IDbTransaction>();

        //    mockDataContext.Stub(x => x.Connection).Return(mockConnection);
        //    mockConnection.Expect(x => x.BeginTransaction(isoLevel)).Return(mockTransaction);

        //    var unitOfWork = new EFUnitOfWork(mockDataContext);
        //    Assert.That(!unitOfWork.IsInTransaction);
        //    unitOfWork.BeginTransaction(IsolationLevel.Snapshot);
        //    Assert.That(unitOfWork.IsInTransaction);

        //    mockDataContext.VerifyAllExpectations();
        //    mockConnection.VerifyAllExpectations();
        //    mockTransaction.VerifyAllExpectations();
        //}

        //[Test]
        //public void BeginTransaction_Throws_InvalidOperationException_When_Transaction_Already_Running()
        //{
        //    var mockDataContext = MockRepository.GenerateMock<IEFSession>();
        //    var mockConnection = MockRepository.GenerateMock<IDbConnection>();
        //    var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

        //    mockDataContext.Stub(x => x.Connection).Return(mockConnection);
        //    mockConnection.Expect(x => x.BeginTransaction(IsolationLevel.Unspecified))
        //                    .IgnoreArguments().Return(mockTransaction);

        //    var unitOfWork = new EFUnitOfWork(mockDataContext);
        //    unitOfWork.BeginTransaction();

        //    Assert.That(unitOfWork.IsInTransaction);
        //    Assert.Throws<InvalidOperationException>(() => unitOfWork.BeginTransaction());

        //    mockDataContext.VerifyAllExpectations();
        //    mockConnection.VerifyAllExpectations();
        //    mockTransaction.VerifyAllExpectations();
        //}

        //[Test]
        //public void Flush_Calls_Underlying_DataContext_SubmitChanges()
        //{
        //    var mockDataContext = MockRepository.GenerateMock<IEFSession>();
        //    mockDataContext.Expect(x => x.SaveChanges());

        //    var unitOfWork = new EFUnitOfWork(mockDataContext);
        //    unitOfWork.Flush();

        //    mockDataContext.VerifyAllExpectations();
        //}

        //[Test]
        //public void TransactionalFlush_Starts_A_Transaction_With_Default_Isolation_And_Commits_When_Flush_Succeeds()
        //{
        //    var mockDataContext = MockRepository.GenerateMock<IEFSession>();
        //    var mockConnection = MockRepository.GenerateMock<IDbConnection>();
        //    var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

        //    mockDataContext.Stub(x => x.Connection).Return(mockConnection);
        //    mockConnection.Expect(x => x.BeginTransaction(IsolationLevel.Unspecified))
        //                  .IgnoreArguments()
        //                  .Return(mockTransaction);

        //    mockDataContext.Expect(x => x.SaveChanges());
        //    mockConnection.Expect(x => x.State).Return(ConnectionState.Closed); //First call should be closed
        //    mockConnection.Expect(x => x.Open());
        //    mockTransaction.Expect(x => x.Commit());
        //    mockConnection.Expect(x => x.State).Return(ConnectionState.Open); //Second call should be open
        //    mockConnection.Expect(x => x.Close());
            

        //    var unitOfWork = new EFUnitOfWork(mockDataContext);
        //    unitOfWork.TransactionalFlush();

        //    mockDataContext.VerifyAllExpectations();
        //    mockConnection.VerifyAllExpectations();
        //    mockTransaction.VerifyAllExpectations();
        //}

        //[Test]
        //public void TransactionalFlush_Starts_A_Transaction_With_Specified_IsolationLevel_And_Commits_When_Flush_Succeeds()
        //{
        //    var mockDataContext = MockRepository.GenerateMock<IEFSession>();
        //    var mockConnection = MockRepository.GenerateMock<IDbConnection>();
        //    var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

        //    mockDataContext.Stub(x => x.Connection).Return(mockConnection);
        //    mockConnection.Expect(x => x.BeginTransaction(IsolationLevel.Unspecified))
        //                  .IgnoreArguments()
        //                  .Return(mockTransaction);
        //    mockDataContext.Expect(x => x.SaveChanges());
        //    mockTransaction.Expect(x => x.Commit());

        //    var unitOfWork = new EFUnitOfWork(mockDataContext);
        //    unitOfWork.TransactionalFlush(IsolationLevel.ReadUncommitted);

        //    mockDataContext.VerifyAllExpectations();
        //    mockConnection.VerifyAllExpectations();
        //    mockTransaction.VerifyAllExpectations();
        //}

        //[Test]
        //public void TransactionalFlush_Rollsback_Transaction_When_Flush_Throws_Exception()
        //{
        //    var mockDataContext = MockRepository.GenerateMock<IEFSession>();
        //    var mockConnection = MockRepository.GenerateMock<IDbConnection>();
        //    var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

        //    mockDataContext.Stub(x => x.Connection).Return(mockConnection);
        //    mockConnection.Expect(x => x.BeginTransaction(IsolationLevel.Unspecified))
        //                  .IgnoreArguments()
        //                  .Return(mockTransaction);
        //    mockDataContext.Expect(x => x.SaveChanges()).Throw(new Exception());
        //    mockTransaction.Expect(x => x.Rollback());

        //    var unitOfWork = new EFUnitOfWork(mockDataContext);
        //    Assert.Throws<Exception>(unitOfWork.TransactionalFlush);

        //    mockDataContext.VerifyAllExpectations();
        //    mockConnection.VerifyAllExpectations();
        //    mockTransaction.VerifyAllExpectations();
        //}

        //[Test]
        //public void TransactionalFlush_Uses_Existing_Transaction_When_Transactional_AlreadyRunning()
        //{
        //    var mockDataContext = MockRepository.GenerateMock<IEFSession>();
        //    var mockConnection = MockRepository.GenerateMock<IDbConnection>();
        //    var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

        //    mockDataContext.Expect(x => x.Connection).Repeat.Any().Return(mockConnection);
        //    mockConnection.Stub(x => x.BeginTransaction(IsolationLevel.Unspecified))
        //                  .IgnoreArguments()
        //                  .Repeat.Once()
        //                  .Return(mockTransaction);


        //    var unitOfWork = new EFUnitOfWork(mockDataContext);

        //    unitOfWork.BeginTransaction();
        //    unitOfWork.TransactionalFlush();

        //    mockDataContext.VerifyAllExpectations();
        //    mockConnection.VerifyAllExpectations();
        //    mockTransaction.VerifyAllExpectations();
        //}

        //[Test]
        //public void Comitting_Transaction_Releases_Transaction_From_UnitOfWork()
        //{
        //    var mockDataContext = MockRepository.GenerateMock<IEFSession>();
        //    var mockConnection = MockRepository.GenerateMock<IDbConnection>();
        //    var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

        //    mockDataContext.Stub(x => x.Connection).Return(mockConnection);
        //    mockConnection.Expect(x => x.BeginTransaction(IsolationLevel.Unspecified))
        //                  .IgnoreArguments()
        //                  .Return(mockTransaction);
        //    mockTransaction.Expect(x => x.Commit());

        //    var unitOfWork = new EFUnitOfWork(mockDataContext);
        //    var transaction = unitOfWork.BeginTransaction();

        //    Assert.That(unitOfWork.IsInTransaction);
        //    transaction.Commit();
        //    Assert.That(!unitOfWork.IsInTransaction);

        //    mockDataContext.VerifyAllExpectations();
        //    mockConnection.VerifyAllExpectations();
        //    mockTransaction.VerifyAllExpectations();
        //}

        //[Test]
        //public void Rollback_Transaction_Releases_Transaction_From_UnitOfWork()
        //{
        //    var mockDataContext = MockRepository.GenerateMock<IEFSession>();
        //    var mockConnection = MockRepository.GenerateMock<IDbConnection>();
        //    var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

        //    mockDataContext.Stub(x => x.Connection).Return(mockConnection);
        //    mockConnection.Expect(x => x.BeginTransaction(IsolationLevel.Unspecified))
        //                  .IgnoreArguments()
        //                  .Return(mockTransaction);
        //    mockTransaction.Expect(x => x.Rollback());

        //    var unitOfWork = new EFUnitOfWork(mockDataContext);
        //    var transaction = unitOfWork.BeginTransaction();

        //    Assert.That(unitOfWork.IsInTransaction);
        //    transaction.Rollback();
        //    Assert.That(!unitOfWork.IsInTransaction);

        //    mockDataContext.VerifyAllExpectations();
        //    mockConnection.VerifyAllExpectations();
        //    mockTransaction.VerifyAllExpectations();
        //}

        //[Test]
        //public void Dispose_UnitOfWork_Disposed_Underlying_Transaction_And_Session()
        //{
        //    var mockDataContext = MockRepository.GenerateMock<IEFSession>();
        //    var mockConnection = MockRepository.GenerateMock<IDbConnection>();
        //    var mockTransaction = MockRepository.GenerateMock<IDbTransaction>();

        //    mockDataContext.Stub(x => x.Connection).Return(mockConnection);
        //    mockConnection.Expect(x => x.BeginTransaction(IsolationLevel.Unspecified))
        //                  .IgnoreArguments()
        //                  .Return(mockTransaction);

        //    mockTransaction.Expect(x => x.Dispose());
        //    mockDataContext.Expect(x => x.Dispose());

        //    using (var unitOfWork = new EFUnitOfWork(mockDataContext))
        //    {
        //        unitOfWork.BeginTransaction();
        //    }

        //    mockDataContext.VerifyAllExpectations();
        //    mockConnection.VerifyAllExpectations();
        //    mockTransaction.VerifyAllExpectations();
        //}
    }
}
