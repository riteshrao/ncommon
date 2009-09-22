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

namespace NCommon.Data.NHibernate
{
    public class NHTransaction : ITransaction
    {
        #region fields
        private bool _disposed;
        private readonly global::NHibernate.ITransaction _transaction;
        #endregion

        #region ctor
        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="NHTransaction"/> instance.
        /// </summary>
        /// <param name="transaction">The underlying NHibernate.ITransaction instance.</param>
        public NHTransaction(global::NHibernate.ITransaction transaction)
        {
            Guard.Against<ArgumentNullException>(transaction == null, "Expected a non null NHibernate.ITransaction instance.");
            _transaction = transaction;
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
        /// Disposes off managed and un-managed resources used by the <see cref="NHTransaction"/> instnace.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_disposed)
                {
                    _transaction.Dispose();
                    _disposed = true;
                }
            }
        }
        #endregion

        #region Implementation of ITransaction
        /// <summary>
        /// Event raised when the transaction has been comitted.
        /// </summary>
        public event EventHandler TransactonComitted;

        /// <summary>
        /// Event raised when the transaction has been rolledback.
        /// </summary>
        public event EventHandler TransactionRolledback;

        /// <summary>
        /// Commits the changes made to the data store.
        /// </summary>
        public void Commit()
        {
            if (_disposed)
                throw new ObjectDisposedException("NHTransaction", "Cannot commit a disposed transaction.");

            _transaction.Commit();
            if (TransactonComitted != null)
                TransactonComitted(this, EventArgs.Empty);
        }

        /// <summary>
        /// Rollsback any changes made.
        /// </summary>
        public void Rollback()
        {
            if (_disposed)
                throw new ObjectDisposedException("NHTransaction", "Cannot rollback a disposed transaction.");

            _transaction.Rollback();
            if (TransactionRolledback != null)
                TransactionRolledback(this, EventArgs.Empty);
        }
        #endregion
    }
}