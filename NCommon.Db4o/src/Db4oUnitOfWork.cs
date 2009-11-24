using System;
using System.Data;
using Db4objects.Db4o;
using NCommon.Data;

namespace NCommon.Db4o
{
    /// <summary>
    /// 
    /// </summary>
    public class Db4oUnitOfWork : IUnitOfWork
    {
        bool _disposed;
        Db4oTransaction _transaction;

        public Db4oUnitOfWork(IObjectContainer container)
        {
            Guard.Against<ArgumentNullException>(container == null, "Expected a non-null IObjectContainer instance.");
            ObjectContainer = container;
        }

        public bool IsInTransaction
        {
            get { return true; } 
        }

        public IObjectContainer ObjectContainer { get; private set; }

        public ITransaction BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.Unspecified);
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            Guard.Against<InvalidOperationException>(_transaction != null,
                                                     "Cannot begin a new transaction while an existing transaction is still running. " +
                                                     "Please commit or rollback the existing transaction before starting a new one.");
            _transaction = new Db4oTransaction(ObjectContainer);
            _transaction.TransactionCommitted += TransactionCommitted;
            _transaction.TransactionRolledback += TransactionRolledback;
            return _transaction;
        }

        public void Flush()
        {
            TransactionalFlush(); //Performs a transactional flush since Db4o works using implicit transactions.
        }

        public void TransactionalFlush()
        {
            TransactionalFlush(IsolationLevel.Unspecified);
        }

        public void TransactionalFlush(IsolationLevel isolationLevel)
        {
            if (!IsInTransaction)
                BeginTransaction();

            try
            {
                _transaction.Commit();
            }
            catch (Exception)
            {
                _transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Handles the <see cref="ITransaction.TransactionRolledback"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransactionRolledback(object sender, EventArgs e)
        {
            Guard.IsEqual<InvalidOperationException>(sender, _transaction,
                        "Expected the sender of TransactionRolledback event to be the transaction that was created by the NHUnitOfWork instance.");
            ReleaseCurrentTransaction();
        }

        /// <summary>
        /// Handles the <see cref="ITransaction.TransactionCommitted"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransactionCommitted(object sender, EventArgs e)
        {
            Guard.IsEqual<InvalidOperationException>(sender, _transaction,
                "Expected the sender of TransactionComitted event to be the transaction that was created by the NHUnitOfWork instance.");
            ReleaseCurrentTransaction();
        }

        /// <summary>
        /// Releases the current transaction in the <see cref="NHUnitOfWork"/> instance.
        /// </summary>
        private void ReleaseCurrentTransaction()
        {
            if (_transaction != null)
            {
                _transaction.TransactionCommitted -= TransactionCommitted;
                _transaction.TransactionRolledback -= TransactionRolledback;
                _transaction.Dispose();
            }
            _transaction = null;
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

        /// <summary>
        /// Disposes off managed resources used by the NHUnitOfWork instance.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            if (_disposed)
                return;

            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            if (ObjectContainer != null)
            {
                ObjectContainer.Dispose();
                ObjectContainer = null;
            }
            _disposed = true;
        }
    }
}