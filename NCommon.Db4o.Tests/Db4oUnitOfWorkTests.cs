using System;
using Db4objects.Db4o;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Db4o.Tests
{
    public class Db4oUnitOfWorkTests
    {
        [Test]
        public void Ctor_Throws_ArgumentNullException_When_ISession_Parameter_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new Db4oUnitOfWork(null));
        }

        [Test]
        public void IsInTransaction_Should_Return_False_When_No_Transaction_Has_Been_Started()
        {
            var mockContainer = MockRepository.GenerateStub<IObjectContainer>();
            var unitOfWork = new Db4oUnitOfWork(mockContainer);

            Assert.That(!unitOfWork.IsInTransaction);
        }

        [Test]
        public void Begin_Transaction_Should_Start_A_New_Transaction_With_Default_IsolationLevel()
        {
            var mockContainer = MockRepository.GenerateMock<IObjectContainer>();
            var unitOfWork = new Db4oUnitOfWork(mockContainer);
            var transaction = unitOfWork.BeginTransaction();

            Assert.That(unitOfWork.IsInTransaction);
            Assert.That(transaction, Is.Not.Null);
            mockContainer.VerifyAllExpectations();
        }

        [Test]
        public void BeginTransaction_Throws_InvalidOperationException_When_Transaction_Already_Running()
        {
            var mockContainer = MockRepository.GenerateStub<IObjectContainer>();
            var unitOfWork = new Db4oUnitOfWork(mockContainer);
            unitOfWork.BeginTransaction();

            Assert.That(unitOfWork.IsInTransaction);
            Assert.Throws<InvalidOperationException>(() => unitOfWork.BeginTransaction());
        }

        [Test]
        public void Flush_Performs_A_Transactional_Commit_When_No_Transaction_Has_Started()
        {
            var mockContainer = MockRepository.GenerateMock<IObjectContainer>();
            var unitOfWork = new Db4oUnitOfWork(mockContainer);
            unitOfWork.Flush();
            mockContainer.AssertWasCalled(x => x.Commit());
            mockContainer.VerifyAllExpectations();
        }

        [Test]
        public void Flush_Does_Nothing_When_A_TransactionHas_Already_Been_Started()
        {
            var mockContainer = MockRepository.GenerateMock<IObjectContainer>();
            var unitOfWork = new Db4oUnitOfWork(mockContainer);
            unitOfWork.BeginTransaction();
            unitOfWork.Flush();
            mockContainer.AssertWasNotCalled(x => x.Commit());
            mockContainer.AssertWasNotCalled(x => x.Rollback());
            mockContainer.VerifyAllExpectations();
        }

        [Test]
        public void TransactionalFlush_Starts_A_Transaction_With_Default_Isolation_And_Commits_When_Flush_Succeeds()
        {
            var mockContainer = MockRepository.GenerateMock<IObjectContainer>();
            var unitOfWork = new Db4oUnitOfWork(mockContainer);
            unitOfWork.TransactionalFlush();
            mockContainer.AssertWasCalled(x => x.Commit());
            mockContainer.VerifyAllExpectations();
        }

        [Test]
        public void TransactionalFlush_Rollsback_Transaction_When_Flush_Throws_Exception()
        {
            var mockContainer = MockRepository.GenerateMock<IObjectContainer>();
            mockContainer.Expect(x => x.Commit())
                .Throw(new ApplicationException());
            var unitOfWork = new Db4oUnitOfWork(mockContainer);
            Assert.Throws<ApplicationException>(unitOfWork.TransactionalFlush);
            mockContainer.AssertWasCalled(x => x.Rollback());
            mockContainer.VerifyAllExpectations();
        }
    }
}