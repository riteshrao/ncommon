using System;
using System.Data;
using Db4objects.Db4o;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.Db4o.Tests
{
    [TestFixture]
    public class Db4oTransactionTests
    {
        [Test]
        public void Ctor_Throws_ArgumentNullException_When_ITransation_Parameter_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new Db4oTransaction(IsolationLevel.Unspecified, null));
        }

        [Test]
        public void Commit_Calls_Commit_On_Underlying_Container()
        {
            var mockContainer = MockRepository.GenerateMock<IObjectContainer>();
            var transaction = new Db4oTransaction(IsolationLevel.Unspecified, mockContainer);
            transaction.Commit();
            mockContainer.AssertWasCalled(x => x.Commit());
            mockContainer.VerifyAllExpectations();
        }

        [Test]
        public void Rollback_Calls_Rollback_On_Underlying_ITransaction()
        {
            var mockContainer = MockRepository.GenerateMock<IObjectContainer>();
            var transaction = new Db4oTransaction(IsolationLevel.Unspecified, mockContainer);
            transaction.Rollback();
            mockContainer.AssertWasCalled(x => x.Rollback());
            mockContainer.VerifyAllExpectations();
        }

        [Test]
        public void Commit_Raises_TransactionComitted_Event()
        {
            var mockContainer = MockRepository.GenerateMock<IObjectContainer>();

            var commitCalled = false;
            var rollbackCalled = false;
            var transaction = new Db4oTransaction(IsolationLevel.Unspecified, mockContainer);
            transaction.TransactionCommitted += delegate { commitCalled = true; };
            transaction.TransactionRolledback += delegate { rollbackCalled = true; };

            transaction.Commit();

            Assert.That(commitCalled);
            Assert.That(!rollbackCalled);
            mockContainer.AssertWasCalled(x => x.Commit());
            mockContainer.AssertWasNotCalled(x => x.Rollback());
            mockContainer.VerifyAllExpectations();
        }

        [Test]
        public void Rollback_Raises_RollbackComitted_Event()
        {
            var mockContainer = MockRepository.GenerateMock<IObjectContainer>();

            var commitCalled = false;
            var rollbackCalled = false;
            var transaction = new Db4oTransaction(IsolationLevel.Unspecified, mockContainer);
            transaction.TransactionCommitted += delegate { commitCalled = true; };
            transaction.TransactionRolledback += delegate { rollbackCalled = true; };

            transaction.Rollback();
            Assert.That(!commitCalled);
            Assert.That(rollbackCalled);

            mockContainer.AssertWasCalled(x => x.Rollback());
            mockContainer.AssertWasNotCalled(x => x.Commit());
            mockContainer.VerifyAllExpectations();
        }
    }
}