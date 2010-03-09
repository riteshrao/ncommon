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
using System.Data;
using NCommon.Extensions;

namespace NCommon.Data.LinqToSql
{
    public class LinqToSqlTransaction : ITransaction
    {
        bool _disposed;
        readonly ICollection<IDbTransaction> _transactions = new HashSet<IDbTransaction>();

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="LinqToSqlTransaction"/>
        /// </summary>
        /// <param name="transaction">The underlying <see cref="IDbTransaction"/></param>
        public LinqToSqlTransaction(IsolationLevel isolationLevel, params IDbTransaction[] transaction)
        {
            Guard.Against<ArgumentNullException>(transaction == null, "Expected a non null IDbTransaction instance.");
            IsolationLevel = isolationLevel;
            transaction.ForEach(tx => _transactions.Add(tx));
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

        public void RegisterTransaction(IDbTransaction transaction)
        {
            _transactions.Add(transaction);
        }

        /// <summary>
        /// Commits the changes made to the data store.
        /// </summary>
        /// <remarks>Implementors MUST raise the <see cref="ITransaction.TransactionCommitted"/> event.</remarks>
        public void Commit()
        {
            if (_disposed)
                throw new ObjectDisposedException("LinqToSqlTransaction", "Cannot commit a disposed transaction.");

            _transactions.ForEach(tx => tx.Commit());
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

            _transactions.ForEach(x => x.Rollback());
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
        /// Disposes off managed and un-managed transactions.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_disposed) return;

            if (_transactions.Count > 0)
            {
                _transactions.ForEach(tx => tx.Dispose());
                _transactions.Clear();
            }
            _disposed = true;
        }

      
    }
}
