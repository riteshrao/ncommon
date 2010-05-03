#region license
//Copyright 2010 Ritesh Rao 

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
using Common.Logging;
using Microsoft.Practices.ServiceLocation;
using NCommon.Extensions;

namespace NCommon.Data.Impl
{
    /// <summary>
    /// Default implementation of <see cref="ITransactionManager"/> interface.
    /// </summary>
    public class TransactionManager : ITransactionManager, IDisposable
    {
        bool _disposed;
        readonly Guid _transactionManagerId = Guid.NewGuid();
        readonly ILog _logger = LogManager.GetLogger<TransactionManager>();
        readonly LinkedList<UnitOfWorkTransaction> _transactions = new LinkedList<UnitOfWorkTransaction>();

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="TransactionManager"/> class.
        /// </summary>
        public TransactionManager()
        {
            _logger.Debug(x => x("New instance of TransactionManager with Id {0} created.", _transactionManagerId));
        }

        /// <summary>
        /// Gets the current <see cref="IUnitOfWork"/> instance.
        /// </summary>
        public IUnitOfWork CurrentUnitOfWork
        {
            get 
            {
                return CurrentTransaction == null ? null : CurrentTransaction.UnitOfWork;
            }
        }

        /// <summary>
        /// Gets the current <see cref="UnitOfWorkTransaction"/> instance.
        /// </summary>
        public UnitOfWorkTransaction CurrentTransaction
        {
            get
            {
                return _transactions.Count == 0 ? null : _transactions.First.Value;
            }
        }

        /// <summary>
        /// Enlists a <see cref="UnitOfWorkScope"/> instance with the transaction manager.
        /// </summary>
        /// <param name="scope">bool. True if the scope should be enlisted in a new transaction, else
        /// false if the scope should participate in the existing transaction</param>
        /// <param name="newTransaction"></param>
        public void EnlistScope(IUnitOfWorkScope scope, bool newTransaction)
        {
            _logger.Info(x => x("Enlisting scope {0} with transaction manager {1}.", scope.ScopeId, _transactionManagerId, newTransaction));

            var uowFactory = ServiceLocator.Current.GetInstance<IUnitOfWorkFactory>();
            if (newTransaction || _transactions.Count == 0)
            {
                _logger.Debug("Either UnitOfWorkScope started as a newTransaction, or no existing transactions started. Creating a new transaction...");
                var txScope = newTransaction 
                                               ? TransactionScopeHelper.CreateNewScope(UnitOfWorkSettings.DefaultIsolation) 
                                               : TransactionScopeHelper.CreateScope(UnitOfWorkSettings.DefaultIsolation);

                var unitOfWork = uowFactory.Create();
                var transaction = new UnitOfWorkTransaction(unitOfWork, txScope);
                transaction.TransactionDisposing += OnTransactionDisposing;
                transaction.EnlistScope(scope);
                _transactions.AddFirst(transaction);
                return;
            }
            CurrentTransaction.EnlistScope(scope);
        }

        void OnTransactionDisposing(UnitOfWorkTransaction transaction)
        {
            _logger.Info(x => x("UnitOfWorkTransaction {0} signalled a disposed. Unregistering transaction from TransactionManager {1}",
                                    transaction.TransactionId, _transactionManagerId));

            transaction.TransactionDisposing -= OnTransactionDisposing;
            var node = _transactions.Find(transaction);
            if (node != null)
                _transactions.Remove(node);
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

            if (disposing)
            {
                _logger.Info(x => x("Disposing off transction manager {0}", _transactionManagerId));
                if (_transactions != null && _transactions.Count > 0)
                {
                    _transactions.ForEach(tx =>
                    {
                        tx.TransactionDisposing -= OnTransactionDisposing;
                        tx.Dispose();
                    });
                    _transactions.Clear();
                }
            }
            _disposed = true;
        }
    }
}