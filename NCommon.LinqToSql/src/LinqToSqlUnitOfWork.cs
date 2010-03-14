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
using System.Data.Common;
using System.Data.Linq;
using System.Linq;
using NCommon.Extensions;

namespace NCommon.Data.LinqToSql
{
    /// <summary>
    /// Implements the <see cref="IUnitOfWork"/> interface to provide an implementation
    /// of a IUnitOfWork that uses NHibernate to query and update the underlying store.
    /// </summary>
    public class LinqToSqlUnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        private LinqToSqlTransaction _transaction;
        readonly LinqToSqlUnitOfWorkSettings _settings;
        readonly IDictionary<Guid, ILinqToSqlSession> _openSessions = new Dictionary<Guid, ILinqToSqlSession>();

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="LinqToSqlUnitOfWork"/> class that uses the specified data  context.
        /// </summary>
        /// <param name="context">The <see cref="DataContext"/> instance that the LinqToSqlUnitOfWork instance uses.</param>
        public LinqToSqlUnitOfWork(LinqToSqlUnitOfWorkSettings settings) 
        {
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
        /// Gets a <see cref="DataContext"/> that can be used to query and update the specified type.
        /// </summary>
        /// <typeparam name="T">The type for which a <see cref="DataContext"/> instance is retrieved.</typeparam>
        /// <returns>A <see cref="DataContext"/> instance that can be used to query and update the specified type.</returns>
        public DataContext GetContext<T>()
        {
            var key = _settings.SessionResolver.GetSessionKeyFor<T>();
            if (_openSessions.ContainsKey(key))
                return _openSessions[key].Context;

            //Opening a new session...
            var session = _settings.SessionResolver.OpenSessionFor<T>();
            _openSessions.Add(key, session);
            if (IsInTransaction)
            {
                if (session.Connection.State != ConnectionState.Open)
                    session.Connection.Open();
                _transaction.RegisterTransaction(session.Connection.BeginTransaction(_transaction.IsolationLevel));
            }
            return session.Context;
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

            _transaction = new LinqToSqlTransaction(isolationLevel, _openSessions.Select(session =>
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
            _openSessions.ForEach(session => session.Value.SubmitChanges());
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
        private void TransactionRolledback(object sender, EventArgs e)
        {
            Guard.IsEqual<InvalidOperationException>(sender, _transaction,
                        "Expected the sender of TransactionRolledback event to be the transaction that was created by the LinqToSqlUnitOfWork instance.");
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
                "Expected the sender of TransactionComitted event to be the transaction that was created by the LintToSqlUnitOfWork instance.");
            ReleaseCurrentTransaction();
        }

        /// <summary>
        /// Releases the current transaction in the <see cref="LinqToSqlUnitOfWork"/> instance.
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
        /// Disposes off manages resources used by the LinqToSqlUnitOfWork instance.
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
            if(_openSessions.Count > 0)
            {
                _openSessions.ForEach(session => session.Value.Dispose());
                _openSessions.Clear();
            }
            _disposed = true;
        }
    }
}
