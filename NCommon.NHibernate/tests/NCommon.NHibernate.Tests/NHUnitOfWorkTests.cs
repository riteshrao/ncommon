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
using NHibernate;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.NHibernate.Tests
{
    /// <summary>
    /// Tests the <see cref="NHUnitOfWork"/> instance.
    /// </summary>
    [TestFixture]
    public class NHUnitOfWorkTests
    {
        [Test]
        public void Ctor_Throws_ArgumentNullException_When_Settings_Parameter_Is_Null ()
        {
            Assert.Throws<ArgumentNullException>(() => new NHUnitOfWork(null));
        }

        [Test]
        public void GetSession_returns_session_from_session_resolver()
        {
            var resolver = MockRepository.GenerateMock<INHSessionResolver>();
            resolver.Expect(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Expect(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateStub<ISession>());

            var unitOfWork = new NHUnitOfWork(new NHUnitOfWorkSettings {SessionResolver = resolver});
            var session = unitOfWork.GetSession<string>();
            Assert.That(session, Is.Not.Null);
            resolver.VerifyAllExpectations();
        }

        [Test]
        public void GetSession_returns_same_session_instance_once_opened()
        {
            var resolver = MockRepository.GenerateMock<INHSessionResolver>();
            resolver.Expect(x => x.GetSessionKeyFor<string>())
                .Return(Guid.NewGuid())
                .Repeat.Twice();

            resolver.Expect(x => x.OpenSessionFor<string>())
                .Return(MockRepository.GenerateStub<ISession>())
                .Repeat.Once();

            var unitOfWork = new NHUnitOfWork(new NHUnitOfWorkSettings { SessionResolver = resolver });
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
            var resolver = MockRepository.GenerateMock<INHSessionResolver>();
            resolver.Expect(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Expect(x => x.GetSessionKeyFor<int>()).Return(Guid.NewGuid());
            resolver.Expect(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateStub<ISession>());
            resolver.Expect(x => x.OpenSessionFor<int>()).Return(MockRepository.GenerateStub<ISession>());

            var unitOfWork = new NHUnitOfWork(new NHUnitOfWorkSettings { SessionResolver = resolver });
            var session1 = unitOfWork.GetSession<string>();
            var session2 = unitOfWork.GetSession<int>();
            Assert.That(session1, Is.Not.Null);
            Assert.That(session2, Is.Not.Null);
            Assert.That(session1, Is.Not.SameAs(session2));
            resolver.VerifyAllExpectations();
        }

        [Test]
        public void GetSession_calls_BeginSession_on_session_being_opened_if_transaction_has_already_started()
        {
            var resolver = MockRepository.GenerateStub<INHSessionResolver>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<ISession>());
            resolver.OpenSessionFor<string>()
                .Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(MockRepository.GenerateMock<global::NHibernate.ITransaction>());

            var unitOfWork = new NHUnitOfWork(new NHUnitOfWorkSettings
            {
                DefaultIsolation = IsolationLevel.ReadCommitted,
                SessionResolver = resolver
            });

            unitOfWork.BeginTransaction();
            var session = unitOfWork.GetSession<string>();
            session.VerifyAllExpectations();
        }

        [Test]
        public void IsInTransaction_Should_Return_False_When_No_Transaction_Exists()
        {
            var unitOfWork = new NHUnitOfWork(new NHUnitOfWorkSettings());
            Assert.That(!unitOfWork.IsInTransaction);
        }

        [Test]
        public void Begin_Transaction_Should_Start_A_New_Transaction_With_Default_IsolationLevel()
        {
            var settings = new NHUnitOfWorkSettings {DefaultIsolation = IsolationLevel.Chaos};
            var unitOfWork = new NHUnitOfWork(settings);
            var transaction = unitOfWork.BeginTransaction();
            Assert.That(transaction.IsolationLevel, Is.EqualTo(settings.DefaultIsolation));
        }

        [Test]
        public void BeginTransaction_Should_Start_A_New_Transaction_With_Specified_IsolatinLevel()
        {
            var settings = new NHUnitOfWorkSettings { DefaultIsolation = IsolationLevel.Chaos };
            var unitOfWork = new NHUnitOfWork(settings);
            var transaction = unitOfWork.BeginTransaction(IsolationLevel.ReadCommitted);
            Assert.That(transaction.IsolationLevel, Is.Not.EqualTo(settings.DefaultIsolation));
            Assert.That(transaction.IsolationLevel, Is.EqualTo(IsolationLevel.ReadCommitted));

        }

        [Test]
        public void BeginTransaction_Throws_InvalidOperationException_When_Transaction_Already_Running()
        {
            var settings = new NHUnitOfWorkSettings();
            var unitOfWork = new NHUnitOfWork(settings);
            unitOfWork.BeginTransaction();
            Assert.Throws<InvalidOperationException>(() => unitOfWork.BeginTransaction());
        }

        [Test]
        public void BeginTransaction_calls_BeginTransaction_on_sessions_already_opened()
        {
            var resolver = MockRepository.GenerateStub<INHSessionResolver>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<ISession>());
            resolver.OpenSessionFor<string>()
                .Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(MockRepository.GenerateMock<global::NHibernate.ITransaction>());

            var unitOfWork = new NHUnitOfWork(new NHUnitOfWorkSettings
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
            var resolver = MockRepository.GenerateStub<INHSessionResolver>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<ISession>());

            var unitOfWork = new NHUnitOfWork(new NHUnitOfWorkSettings {SessionResolver = resolver});
            var session = unitOfWork.GetSession<string>();
            unitOfWork.Flush();
            session.AssertWasCalled(x => x.Flush());
        }

        [Test]
        public void TransactionalFlush_Starts_A_Transaction_With_Default_Isolation_And_Commits_When_Flush_Succeeds()
        {
            var resolver = MockRepository.GenerateStub<INHSessionResolver>();
            var transaction = MockRepository.GenerateMock<global::NHibernate.ITransaction>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<ISession>());
            resolver.OpenSessionFor<string>()
                .Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(transaction);

            var unitOfWork = new NHUnitOfWork(new NHUnitOfWorkSettings
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
            var resolver = MockRepository.GenerateStub<INHSessionResolver>();
            var transaction = MockRepository.GenerateMock<global::NHibernate.ITransaction>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<ISession>());
            resolver.OpenSessionFor<string>()
                .Expect(x => x.BeginTransaction(IsolationLevel.Serializable))
                .Return(transaction);

            var unitOfWork = new NHUnitOfWork(new NHUnitOfWorkSettings
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
            var resolver = MockRepository.GenerateStub<INHSessionResolver>();
            var transaction = MockRepository.GenerateMock<global::NHibernate.ITransaction>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<ISession>());
            resolver.OpenSessionFor<string>()
                .Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(transaction);
            transaction.Expect(x => x.Commit()).Throw(new ApplicationException());

            var unitOfWork = new NHUnitOfWork(new NHUnitOfWorkSettings
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
            var resolver = MockRepository.GenerateStub<INHSessionResolver>();
            var transaction = MockRepository.GenerateMock<global::NHibernate.ITransaction>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<ISession>());
            resolver.OpenSessionFor<string>()
                .Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(transaction)
                .Repeat.Once();

            var unitOfWork = new NHUnitOfWork(new NHUnitOfWorkSettings
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
            var resolver = MockRepository.GenerateStub<INHSessionResolver>();
            var transaction = MockRepository.GenerateMock<global::NHibernate.ITransaction>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<ISession>());
            resolver.OpenSessionFor<string>()
                .Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(transaction);

            var unitOfWork = new NHUnitOfWork(new NHUnitOfWorkSettings
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
            var resolver = MockRepository.GenerateStub<INHSessionResolver>();
            var transaction = MockRepository.GenerateMock<global::NHibernate.ITransaction>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<ISession>());
            resolver.OpenSessionFor<string>()
                .Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(transaction);

            var unitOfWork = new NHUnitOfWork(new NHUnitOfWorkSettings
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
        public void disposing_NHUnitOfWork_disposes_off_open_transactions_and_open_sessions()
        {
            var resolver = MockRepository.GenerateStub<INHSessionResolver>();
            var transaction = MockRepository.GenerateMock<global::NHibernate.ITransaction>();
            resolver.Stub(x => x.GetSessionKeyFor<string>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<string>()).Return(MockRepository.GenerateMock<ISession>());
            resolver.OpenSessionFor<string>()
                .Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(transaction);

            resolver.Stub(x => x.GetSessionKeyFor<int>()).Return(Guid.NewGuid());
            resolver.Stub(x => x.OpenSessionFor<int>()).Return(MockRepository.GenerateMock<ISession>());
            resolver.OpenSessionFor<int>()
                .Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
                .Return(transaction);

            var unitOfWork = new NHUnitOfWork(new NHUnitOfWorkSettings
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
    }
}
