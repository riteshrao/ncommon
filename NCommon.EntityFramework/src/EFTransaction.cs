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

namespace NCommon.Data.EntityFramework
{
    public class EFTransaction : ITransaction
    {
        private bool _disposed;
        private IDbTransaction _transaction;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="EFTransaction"/> instance.
        /// </summary>
        /// <param name="transaction"></param>
        public EFTransaction(IsolationLevel isolationLevel, IDbTransaction transaction)
        {
            Guard.Against<ArgumentNullException>(transaction == null, "Expected a non-null DbTransaction instance.");
            IsolationLevel = isolationLevel;
            _transaction = transaction;
        }

        /// <summary>
        /// Event raised when the transaction has been comitted.
        /// </summary>
        public event EventHandler TransactionCommitted;

        /// <summary>
        /// Event raised when the transaction has been rolledback.
        /// </summary>
        public event EventHandler TransactionRolledback;

        public IsolationLevel IsolationLevel { get; private set; }

        /// <summary>
        /// Commits the changes made to the data store.
        /// </summary>
        /// <remarks>Implementors MUST raise the <see cref="ITransaction.TransactionCommitted"/> event.</remarks>
        public void Commit()
        {
            if (_disposed)
                throw new ObjectDisposedException("EFTransaction", "Cannot commit a disposed transaction.");
            _transaction.Commit();
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
                throw new ObjectDisposedException("EFTransaction", "Cannot rollback a disposed transaction.");

            _transaction.Rollback();
            if (TransactionRolledback != null)
                TransactionRolledback(this, EventArgs.Empty);
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
        /// Disposes off managed and un-managed resources used.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_disposed)
                {
                    _transaction.Dispose();
                    _transaction = null;
                    _disposed = true;
                }
            }
        }
    }
}
