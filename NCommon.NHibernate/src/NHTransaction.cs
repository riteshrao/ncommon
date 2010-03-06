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
using System.Collections.Generic;
using NCommon.Extensions;

namespace NCommon.Data.NHibernate
{
    public class NHTransaction : ITransaction
    {
        bool _disposed;
        readonly ICollection<global::NHibernate.ITransaction> _transactions = new HashSet<global::NHibernate.ITransaction>();

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="NHTransaction"/> instance.
        /// </summary>
        /// <param name="transactions">The underlying NHibernate.ITransaction instance.</param>
        public NHTransaction(params global::NHibernate.ITransaction[] transactions)
        {
            transactions.ForEach(tx => _transactions.Add(tx));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        public void RegisterNHTransaction(global::NHibernate.ITransaction transaction)
        {
            _transactions.Add(transaction);
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
        /// Disposes off managed and un-managed resources used by the <see cref="NHTransaction"/> instnace.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!disposing) 
                return;

            if (_disposed) 
                return;

            if (_transactions.Count > 0)
            {
                _transactions.ForEach(tx => tx.Dispose());
                _transactions.Clear();     
            }
            _disposed = true;
        }

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
        public void Commit()
        {
            if (_disposed)
                throw new ObjectDisposedException("NHTransaction", "Cannot commit a disposed transaction.");
            _transactions.ForEach(tx => tx.Commit());
            if (TransactionCommitted != null)
                TransactionCommitted(this, EventArgs.Empty);
        }

        /// <summary>
        /// Rollsback any changes made.
        /// </summary>
        public void Rollback()
        {
            if (_disposed)
                throw new ObjectDisposedException("NHTransaction", "Cannot rollback a disposed transaction.");

            _transactions.ForEach(tx => tx.Rollback());
            if (TransactionRolledback != null)
                TransactionRolledback(this, EventArgs.Empty);
        }
    }
}