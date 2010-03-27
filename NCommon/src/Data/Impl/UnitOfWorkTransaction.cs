using System;
using System.Collections.Generic;
using System.Transactions;
using NCommon.Extensions;

namespace NCommon.Data.Impl
{
    /// <summary>
    /// Encapsutes a unit of work transaction.
    /// </summary>
    public class UnitOfWorkTransaction : IDisposable
    {
        bool _disposed;
        TransactionScope _transaction;
        IUnitOfWork _unitOfWork;
        IList<IUnitOfWorkScope> _attachedScopes = new List<IUnitOfWorkScope>();

        ///<summary>
        /// Raised when the transaction is disposing.
        ///</summary>
        public event Action<UnitOfWorkTransaction> TransactionDisposing;

        ///<summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="UnitOfWorkTransaction"/> class.
        ///</summary>
        ///<param name="unitOfWork">The <see cref="IUnitOfWork"/> instance managed by the 
        /// <see cref="UnitOfWorkTransaction"/> instance.</param>
        ///<param name="transaction">The <see cref="TransactionScope"/> instance managed by 
        /// the <see cref="UnitOfWorkTransaction"/> instance.</param>
        public UnitOfWorkTransaction(IUnitOfWork unitOfWork, TransactionScope transaction)
        {
            Guard.Against<ArgumentNullException>(unitOfWork == null,
                                                 "Expected a non-null UnitOfWork instance.");
            Guard.Against<ArgumentNullException>(transaction == null,
                                                 "Expected a non-null TransactionScope instance.");
            _unitOfWork = unitOfWork;
            _transaction = transaction;
        }

        /// <summary>
        /// Gets the <see cref="IUnitOfWork"/> instance managed by the 
        /// <see cref="UnitOfWorkTransaction"/> instance.
        /// </summary>
        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        /// <summary>
        /// Attaches a <see cref="UnitOfWorkScope"/> instance to the 
        /// <see cref="UnitOfWorkTransaction"/> instance.
        /// </summary>
        /// <param name="scope">The <see cref="UnitOfWorkScope"/> instance to attach.</param>
        public void EnlistScope(IUnitOfWorkScope scope)
        {
            Guard.Against<ArgumentNullException>(scope == null,
                                                 "Expected a non-null IUnitOfWorkScope instance.");
            _attachedScopes.Add(scope);
            scope.ScopeComitting += OnScopeCommitting;
            scope.ScopeRollingback += OnScopeRollingBack;
        }

        /// <summary>
        /// Callback executed when an enlisted scope has comitted.
        /// </summary>
        void OnScopeCommitting(IUnitOfWorkScope scope)
        {
            Guard.Against<ObjectDisposedException>(_disposed,
                                                   "The transaction attached to the scope has already been disposed.");
           if (!_attachedScopes.Contains(scope))
           {
               Dispose();
               throw new InvalidOperationException("The scope being comitted is not attached to the current transaction.");
           }
            scope.ScopeComitting -= OnScopeCommitting;
            scope.ScopeRollingback -= OnScopeRollingBack;
            scope.Complete();
            _attachedScopes.Remove(scope);
            if (_attachedScopes.Count == 0)
            {
                try
                {
                    _unitOfWork.Flush();
                    _transaction.Complete();
                }
                finally
                {
                    Dispose(); //Dispose the transaction after comitted.
                }
            }
        }

        /// <summary>
        /// Callback executed when an enlisted scope is rolledback.
        /// </summary>
        void OnScopeRollingBack(IUnitOfWorkScope scope)
        {
            Guard.Against<ObjectDisposedException>(_disposed,
                                                   "The transaction attached to the scope has already been disposed.");
            scope.ScopeComitting -= OnScopeCommitting;
            scope.ScopeRollingback -= OnScopeRollingBack;
            _attachedScopes.Remove(scope);
            Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (TransactionDisposing != null)
                    TransactionDisposing(this);

                if (_unitOfWork != null)
                    _unitOfWork.Dispose();

                if (_transaction != null)
                    _transaction.Dispose();

                if (_attachedScopes != null && _attachedScopes.Count > 0)
                {
                    _attachedScopes.ForEach(scope =>
                    {
                        scope.ScopeComitting -= OnScopeCommitting;
                        scope.ScopeRollingback -= OnScopeRollingBack;
                        scope.Complete();
                    });
                    _attachedScopes.Clear();     
                }
            }
            TransactionDisposing = null;
            _unitOfWork = null;
            _transaction = null;
            _attachedScopes = null;
            _disposed = true;
        }
    }
}