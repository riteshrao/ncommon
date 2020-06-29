using System;
using System.Collections.Generic;
using NCommon.DataServices.Transactions;
using Rhino.Mocks;

namespace NCommon.Tests.Data
{
    public class FakeTransactionManager : IUnitOfWorkTransactionManager
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
}