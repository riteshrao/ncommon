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

namespace NCommon.Data.LinqToSql
{
    public class LinqToSqlTransaction : ITransaction
    {
        #region fields
        private bool _disposed;
        private readonly IDbTransaction _internalTransaction;
        #endregion

        #region ctor
        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="LinqToSqlTransaction"/>
        /// </summary>
        /// <param name="transaction">The underlying <see cref="IDbTransaction"/></param>
        public LinqToSqlTransaction(IDbTransaction transaction)
        {
            Guard.Against<ArgumentNullException>(transaction == null, "Expected a non null IDbTransaction instance.");
            _internalTransaction = transaction;
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
        /// Disposes off managed and un-managed transactions.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose (bool disposing)
        {
            if (disposing)
            {
                if (!_disposed)
                {
                    _internalTransaction.Dispose();
                    _disposed = true;
                }
            }
        }
        #endregion

        #region Implementation of ITransaction
        /// <summary>
        /// Event raised when the transaction has been comitted.
        /// </summary>
        public event EventHandler TransactionCommitted;

        /// <summary>
        /// Event raised when the transaction has been rolledback.
        /// </summary>
        public event EventHandler TransactionRolledback;

        /// <summary>
        /// Commits the changes made to the data store.
        /// </summary>
        /// <remarks>Implementors MUST raise the <see cref="ITransaction.TransactionCommitted"/> event.</remarks>
        public void Commit()
        {
            if (_disposed)
                throw new ObjectDisposedException("LinqToSqlTransaction", "Cannot commit a disposed transaction.");

            _internalTransaction.Commit();
            if (TransactionCommitted != null)
                TransactionCommitted(this, EventArgs.Empty);
        }

        /// <summary>
        /// Rollsback any changes made.
        /// </summary>
        /// <remarks>Implementors MUST raise the <see cref="ITransaction.TransactionRolledback"/> event.</remarks>
        public void Rollback()
        {
            if (_disposed)
                throw new ObjectDisposedException("LinqToSqlTransaction", "Cannot rollback a disposed transaction.");

            _internalTransaction.Rollback();
            if (TransactionRolledback != null)
                TransactionRolledback(this, EventArgs.Empty);
        }
        #endregion
    }
}
