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
using Db4objects.Db4o;

namespace NCommon.Data.Db4o
{
    /// <summary>
    /// Implementation of <see cref="ITransaction"/> interface used by <see cref="Db4oUnitOfWork"/>.
    /// </summary>
    public class Db4oTransaction : ITransaction
    {
        bool _disposed;
        readonly IsolationLevel _isolationLevel;
        readonly IObjectContainer _contianer;

        /// <summary>
        /// Event raised when the transaction is committed.
        /// </summary>
        public event EventHandler TransactionCommitted;

        /// <summary>
        /// Event raised when the transaction is rolledback.
        /// </summary>
        public event EventHandler TransactionRolledback;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="Db4oTransaction"/> class.
        /// </summary>
        /// <param name="isolationLevel"><see cref="IsolationLevel"/>. The isolation level of the transaction.</param>
        /// <param name="container">An <see cref="IObjectContainer"/> instance.</param>
        public Db4oTransaction(IsolationLevel isolationLevel, IObjectContainer container)
        {
            Guard.Against<ArgumentNullException>(container == null,
                                                 "Expected a non-null IObjectContainer instance.");
            _isolationLevel = isolationLevel;
            _contianer = container;
        }

        /// <summary>
        /// The isolation level of the transaction.
        /// </summary>
        public IsolationLevel IsolationLevel
        {
            get { return _isolationLevel; }
        }

        /// <summary>
        /// Commits the changes made to the data store.
        /// </summary>
        public void Commit()
        {
            if (_disposed)
                throw new ObjectDisposedException("Db4oTransaction", "Cannot commit a disposed transaction.");
            _contianer.Commit();
            if (TransactionCommitted != null)
                TransactionCommitted(this, EventArgs.Empty);
        }

        /// <summary>
        /// Rollsback any changes made.
        /// </summary>
        public void Rollback()
        {
            if (_disposed)
                throw new ObjectDisposedException("Db4oTransaction", "Cannot rollback a disposed transaction.");
            _contianer.Rollback();
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

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing && _contianer != null)
                _contianer.Dispose();
            _disposed = true;
        }
    }
}