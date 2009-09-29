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
using System.Data.Objects;

namespace NCommon.Data.EntityFramework
{
    /// <summary>
    /// Implements the <see cref="IUnitOfWork"/> interface to provide an implementation
    /// of a IUnitOfWork that uses Entity Framework to query and update the underlying store.
    /// </summary>
    public class EFUnitOfWork : IUnitOfWork
    {
        #region fields
        private bool _disposed;
        private IEFSession _session;
        private EFTransaction _transaction;
        #endregion

        #region ctor
        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="EFUnitOfWork"/> class that uses the specified object context.
        /// </summary>
        /// <param name="session">The <see cref="IEFSession"/> instance that the EFUnitOfWork instance uses.</param>
        public EFUnitOfWork (IEFSession session)
        {
            Guard.Against<ArgumentNullException>(session == null, "Expected a non-null ObjectContext instance.");
            _session = session;
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the <see cref="ObjectContext"/> bound with the EFUnitOfWork instance.
        /// </summary>
        public ObjectContext Context
        {
            get { return _session.Context; }
        }
        #endregion

        #region Implementation of IDisposable
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
        /// Disposes off the managed and unmanaged resources used.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_disposed) return;

            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
            if (_session != null)
            {
                _session.Dispose();
                _session = null;
            }
            _disposed = true;
        }
        #endregion

        #region Implementation of IUnitOfWork
        /// <summary>
        /// Gets a boolean value indicating whether the current unit of work is running under
        /// a transaction.
        /// </summary>
        public bool IsInTransaction
        {
            get { return _transaction != null; }
        }

        /// <summary>
        /// Instructs the <see cref="IUnitOfWork"/> instance to begin a new transaction.
        /// </summary>
        /// <returns></returns>
        public ITransaction BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// Instructs the <see cref="IUnitOfWork"/> instance to begin a new transaction
        /// with the specified isolation level.
        /// </summary>
        /// <param name="isolationLevel">One of the values of <see cref="IsolationLevel"/>
        /// that specifies the isolation level of the transaction.</param>
        /// <returns></returns>
        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            Guard.Against<InvalidOperationException>(_transaction != null,
                                                     "Cannot begin a new transaction while an existing transaction is still running. " +
                                                     "Please commit or rollback the existing transaction before starting a new one.");

            if (_session.Connection.State != ConnectionState.Open)
                _session.Connection.Open();

            IDbTransaction transaction = _session.Connection.BeginTransaction(isolationLevel);
            _transaction = new EFTransaction(transaction);
            _transaction.TransactionCommitted += TransactionCommitted;
            _transaction.TransactionRolledback += TransactionRolledback;
            return _transaction;
        }

        /// <summary>
        /// Flushes the changes made in the unit of work to the data store.
        /// </summary>
        public void Flush()
        {
            _session.SaveChanges();
        }

        /// <summary>
        /// Flushes the changes made in the unit of work to the data store
        /// within a transaction.
        /// </summary>
        public void TransactionalFlush()
        {
            TransactionalFlush(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// Flushes the changes made in the unit of work to the data store
        /// within a transaction with the specified isolation level.
        /// </summary>
        /// <param name="isolationLevel"></param>
        public void TransactionalFlush(IsolationLevel isolationLevel)
        {
            // Start a transaction if one isn't already running.
            if (!IsInTransaction)
                BeginTransaction(isolationLevel);
            try
            {
                _session.SaveChanges();
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Handles the <see cref="ITransaction.TransactionRolledback"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransactionRolledback(object sender, EventArgs e)
        {
            Guard.IsEqual<InvalidOperationException>(sender, _transaction,
                        "Expected the sender of TransactionRolledback event to be the transaction that was created by the EFUnitOfWork instance.");
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
                "Expected the sender of TransactionComitted event to be the transaction that was created by the EFUnitOfWork instance.");
            ReleaseCurrentTransaction();
        }

        /// <summary>
        /// Releases the current transaction in the <see cref="EFTransaction"/> instance.
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

            //Closing the connection once the transaction has completed.
            if (_session.Connection.State == ConnectionState.Open)
                _session.Connection.Close();
        }
        #endregion
    }
}
