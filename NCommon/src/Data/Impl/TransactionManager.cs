using System;
using System.Collections.Generic;
using System.Transactions;
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
        readonly LinkedList<UnitOfWorkTransaction> _transactions = new LinkedList<UnitOfWorkTransaction>();

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
            var uowFactory = ServiceLocator.Current.GetInstance<IUnitOfWorkFactory>();
            if (newTransaction || _transactions.Count == 0)
            {
                TransactionScope txScope = null;
                txScope = newTransaction 
                    ? TransactionScopeHelper.CreateNewScope(UnitOfWorkConfiguration.DefaultIsolation) 
                    : TransactionScopeHelper.CreateScope(UnitOfWorkConfiguration.DefaultIsolation);

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
            transaction.TransactionDisposing -= OnTransactionDisposing;
            var node = _transactions.Find(transaction);
            _transactions.Remove(node);
        }

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
        }
    }
}