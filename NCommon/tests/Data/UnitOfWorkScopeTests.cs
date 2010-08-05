using System;
using System.Collections.Generic;
using NCommon.Data;
using NCommon.Data.Impl;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Tests.Data
{
    public class FakeTransactionManager : ITransactionManager
    {
        readonly IDictionary<Guid, int> _comittedScopes = new Dictionary<Guid, int>();
        readonly IDictionary<Guid, int> _rolledbackScopes = new Dictionary<Guid, int>();
        readonly IUnitOfWork _mockUnitOfWork = MockRepository.GenerateStub<IUnitOfWork>();

        public IUnitOfWork CurrentUnitOfWork
        {
            get { return _mockUnitOfWork; }
        }

        public Action<IUnitOfWorkScope> ScopeCommitAction { get; set; }

        public Action<IUnitOfWorkScope> ScopeRollbackAction { get; set; }

        public void EnlistScope(IUnitOfWorkScope scope, TransactionMode mode)
        {
            scope.ScopeComitting += OnScopeCommitting;
            scope.ScopeRollingback += OnScopeRollingback;
        }

        public int CommitCount(Guid scopeId)
        {
            if (_comittedScopes.ContainsKey(scopeId))
                return _comittedScopes[scopeId];
            return 0;
        }

        public int RollbackCount(Guid scopeId)
        {
            if (_rolledbackScopes.ContainsKey(scopeId))
                return _rolledbackScopes[scopeId];
            return 0;
        }

        public void ResetCounters()
        {
            _comittedScopes.Clear();
            _rolledbackScopes.Clear();
        }

        void OnScopeCommitting(IUnitOfWorkScope scope)
        {
            IncrementCommit(scope.ScopeId);
            if (ScopeCommitAction != null)
                ScopeCommitAction(scope);
            scope.ScopeComitting -= OnScopeCommitting;
        }

        void OnScopeRollingback(IUnitOfWorkScope scope)
        {
            IncrementRollback(scope.ScopeId);
            if (ScopeRollbackAction != null)
                ScopeRollbackAction(scope);
            scope.ScopeRollingback -= OnScopeRollingback;
        }

        void IncrementCommit(Guid scopeId)
        {
            if (!_comittedScopes.ContainsKey(scopeId))
                _comittedScopes.Add(scopeId, 0);
            _comittedScopes[scopeId] = _comittedScopes[scopeId] + 1;
        }

        void IncrementRollback(Guid scopeId)
        {
            if (!_rolledbackScopes.ContainsKey(scopeId))
                _rolledbackScopes.Add(scopeId, 0);
            _rolledbackScopes[scopeId] = _rolledbackScopes[scopeId] + 1;
        }

        public void Dispose()
        {
            //Do nothing...
        }
    }

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