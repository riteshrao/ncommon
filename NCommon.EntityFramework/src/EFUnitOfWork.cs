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
using System.Data.Objects;
using System.Linq;
using NCommon.Extensions;

namespace NCommon.Data.EntityFramework
{
    /// <summary>
    /// Implements the <see cref="IUnitOfWork"/> interface to provide an implementation
    /// of a IUnitOfWork that uses Entity Framework to query and update the underlying store.
    /// </summary>
    public class EFUnitOfWork : IUnitOfWork
    {
        bool _disposed;
        EFTransaction _transaction;
        readonly EFUnitOfWorkSettings _settings;
        readonly IDictionary<Guid, IEFSession> _openSessions = new Dictionary<Guid, IEFSession>();

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="EFUnitOfWork"/> class that uses the specified object context.
        /// </summary>
        /// <param name="settings">An instance of <see cref="EFUnitOfWorkSettings"/> that contains settings for
        /// Entity Framework unit of work instances.</param>
        public EFUnitOfWork(EFUnitOfWorkSettings settings)
        {
            Guard.Against<ArgumentNullException>(settings == null,
                                                 "Expected a non-null EFUnitOfWorkSettings instance.");
            _settings = settings;
        }

        /// <summary>
        /// Gets a boolean value indicating whether the current unit of work is running under
        /// a transaction.
        /// </summary>
        public bool IsInTransaction
        {
            get { return _transaction != null; }
        }

        /// <summary>
        /// Gets a <see cref="IEFSession"/> that can be used to query and update the specified type.
        /// </summary>
        /// <typeparam name="T">The type for which an <see cref="IEFSession"/> should be returned.</typeparam>
        /// <returns>An <see cref="IEFSession"/> that can be used to query and update the specified type.</returns>
        public IEFSession GetSession<T>()
        {
            var sessionKey = _settings.SessionResolver.GetSessionKeyFor<T>();
            if (_openSessions.ContainsKey(sessionKey))
                return _openSessions[sessionKey];

            //Opening a new session...
            var session = _settings.SessionResolver.OpenSessionFor<T>();
            _openSessions.Add(sessionKey, session);
            if (IsInTransaction)
            {
                if (session.Connection.State != ConnectionState.Open)
                    session.Connection.Open();
                _transaction.RegisterTransaction(session.Connection.BeginTransaction(_transaction.IsolationLevel));
            }
            return session;
        }

        /// <summary>
        /// Instructs the <see cref="IUnitOfWork"/> instance to begin a new transaction.
        /// </summary>
        /// <returns></returns>
        public ITransaction BeginTransaction()
        {
            return BeginTransaction(_settings.DefaultIsolation);
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

            _transaction = new EFTransaction(isolationLevel, _openSessions.Select(session =>
            {
                if (session.Value.Connection.State != ConnectionState.Open)
                    session.Value.Connection.Open();
                return session.Value.Connection.BeginTransaction(isolationLevel);
            }).ToArray());

            _transaction.TransactionCommitted += TransactionCommitted;
            _transaction.TransactionRolledback += TransactionRolledback;
            return _transaction;
        }

        /// <summary>
        /// Flushes the changes made in the unit of work to the data store.
        /// </summary>
        public void Flush()
        {
            _openSessions.ForEach(session => session.Value.SaveChanges());
        }

        /// <summary>
        /// Flushes the changes made in the unit of work to the data store
        /// within a transaction.
        /// </summary>
        public void TransactionalFlush()
        {
            TransactionalFlush(_settings.DefaultIsolation);
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
                Flush();
                _transaction.Commit();
            }
            catch
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
        void TransactionRolledback(object sender, EventArgs e)
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
        void TransactionCommitted(object sender, EventArgs e)
        {
            Guard.IsEqual<InvalidOperationException>(sender, _transaction,
                                                     "Expected the sender of TransactionComitted event to be the transaction that was created by the EFUnitOfWork instance.");
            ReleaseCurrentTransaction();
        }

        /// <summary>
        /// Releases the current transaction in the <see cref="EFTransaction"/> instance.
        /// </summary>
        void ReleaseCurrentTransaction()
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
        /// Disposes off the managed and unmanaged resources used.
        /// </summary>
        /// <param name="disposing"></param>
        void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_disposed) return;

            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
            if(_openSessions.Count > 0)
            {
                _openSessions.ForEach(session => session.Value.Dispose());
                _openSessions.Clear();
            }
            _disposed = true;
        }
    }
}