using System;
using NCommon.DataServices.Transactions;
using NCommon.DataServices.Transactions;
using NUnit.Framework;

namespace NCommon.Tests.Data
{
    [TestFixture]
    public class Given_auto_commit_is_disabled
    {
        readonly FakeTransactionManager _transactionManager = new FakeTransactionManager();

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            UnitOfWorkManager.SetTransactionManagerProvider(() => _transactionManager);
        }

        [TearDown]
        public void TestTearDown()
        {
            _transactionManager.ResetCounters();
            _transactionManager.ScopeCommitAction = null;
            _transactionManager.ScopeRollbackAction = null;
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            UnitOfWorkManager.SetTransactionManagerProvider(null);
        }

        [Test]
        public void Should_not_auto_commit_when_scope_is_disposed()
        {
            var scopeId = Guid.Empty;
            using (var scope = new UnitOfWorkScope())
            {
                scopeId = scope.ScopeId;
                //Simulating a rollback by not calling scope.Commit()
            }
            Assert.That(_transactionManager.CommitCount(scopeId), Is.EqualTo(0));
            Assert.That(_transactionManager.RollbackCount(scopeId), Is.EqualTo(1));
        }

        [Test]
        public void Should_raise_commit_signal_when_commit_called()
        {
            var scopeId = Guid.Empty;
            _transactionManager.ScopeCommitAction = committingScope => committingScope.Complete();
            using (var scope = new UnitOfWorkScope())
            {
                scopeId = scope.ScopeId;
                scope.Commit();
            }
            Assert.That(_transactionManager.CommitCount(scopeId), Is.EqualTo(1));
            Assert.That(_transactionManager.RollbackCount(scopeId), Is.EqualTo(0));
        }

        [Test]
        public void Should_raise_rollback_signal_if_commit_failed()
        {
            var scopeId = Guid.Empty;
            _transactionManager.ScopeCommitAction = comittingScope => { throw new InvalidOperationException(); };
            using (var scope = new UnitOfWorkScope())
            {
                scopeId = scope.ScopeId;
                Assert.Throws<InvalidOperationException>(scope.Commit);
            }
            Assert.That(_transactionManager.CommitCount(scopeId), Is.EqualTo(1));
            Assert.That(_transactionManager.RollbackCount(scopeId), Is.EqualTo(1));
        }
    }

    [TestFixture]
    public class Given_auto_commit_is_enabed
    {
        readonly FakeTransactionManager _transactionManager = new FakeTransactionManager();

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            UnitOfWorkManager.SetTransactionManagerProvider(() => _transactionManager);
            UnitOfWorkSettings.AutoCompleteScope = true;
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            UnitOfWorkManager.SetTransactionManagerProvider(null);
            UnitOfWorkSettings.AutoCompleteScope = false;
        }

        [TearDown]
        public void TestTearDown()
        {
            _transactionManager.ResetCounters();
            _transactionManager.ScopeCommitAction = null;
            _transactionManager.ScopeRollbackAction = null;
        }

        [Test]
        public void Should_auto_commit_scope_when_scope_is_disposed()
        {
            var scopeId = Guid.Empty;
            _transactionManager.ScopeCommitAction = scope => scope.Complete();
            using (var scope = new UnitOfWorkScope())
            {
                scopeId = scope.ScopeId;
                //Simulating a dispose here... Scope should signal an auto-commit.
            }
            Assert.That(_transactionManager.CommitCount(scopeId), Is.EqualTo(1));
            Assert.That(_transactionManager.RollbackCount(scopeId), Is.EqualTo(0));
        }

        [Test]
        public void Should_not_attempt_auto_commit_if_explicitly_comitted()
        {
            var scopeId = Guid.Empty;
            _transactionManager.ScopeCommitAction = scope => scope.Complete();
            using (var scope = new UnitOfWorkScope())
            {
                scopeId = scope.ScopeId;
                scope.Commit();
            }
            Assert.That(_transactionManager.CommitCount(scopeId), Is.EqualTo(1));
            Assert.That(_transactionManager.RollbackCount(scopeId), Is.EqualTo(0));
        }

        [Test]
        public void Should_not_attempt_auto_commit_if_excplicit_commit_failed()
        {
            var scopeId = Guid.Empty;
            _transactionManager.ScopeCommitAction = scope => { throw new ApplicationException(); };
            _transactionManager.ScopeRollbackAction = scope => scope.Complete();
            using (var scope = new UnitOfWorkScope())
            {
                scopeId = scope.ScopeId;
                Assert.Throws<ApplicationException>(scope.Commit);
            }
            Assert.That(_transactionManager.CommitCount(scopeId), Is.EqualTo(1));
            Assert.That(_transactionManager.RollbackCount(scopeId), Is.EqualTo(1));
        }

        [Test]
        public void Should_signal_rollback_if_auto_commit_failed()
        {
            var scopeId = Guid.Empty;
            //Not calling Complete on scope simulates a failure on auto commit.
            _transactionManager.ScopeRollbackAction = scope => scope.Complete();
            using (var scope = new UnitOfWorkScope())
            {
                scopeId = scope.ScopeId;
            }
            Assert.That(_transactionManager.CommitCount(scopeId), Is.EqualTo(1));
            Assert.That(_transactionManager.RollbackCount(scopeId), Is.EqualTo(0)); //Rollback should not be signalled because auto commit fails.
        }
    }
}