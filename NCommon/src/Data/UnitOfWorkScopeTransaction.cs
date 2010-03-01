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
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using NCommon.State;

namespace NCommon.Data
{
	/// <summary>
	/// The <see cref="UnitOfWorkScopeTransaction"/> identifies a unique transaciton that can
	/// be shared by multiple <see cref="UnitOfWorkScope"/> instances.
	/// </summary>
	public class UnitOfWorkScopeTransaction : IDisposable
	{
		readonly Stack<UnitOfWorkScope> _attachedScopes;
		readonly IsolationLevel _isolationLevel;
		readonly ITransaction _runningTransaction;
		readonly Guid _transactionId;
		readonly IUnitOfWork _unitOfWork;
		bool _disposed;
		bool _transactionRolledback;
	    const string CurrentTransacitonsKey = "UnitOfWorkScopeTransactions.Key";

		/// <summary>
		/// Overloaded Constructor.
		/// Creates a new instance of the <see cref="UnitOfWorkScopeTransaction"/> that takes in a 
		/// <see cref="IUnitOfWorkFactory"/> instance that is responsible for creating instances of <see cref="IUnitOfWork"/> and
		/// a <see cref="IDbConnection"/> that is used by the instance to connect to the data store.
		/// </summary>
		/// <param name="unitOfWorkFactory">The <see cref="IUnitOfWorkFactory"/> implementation that is responsible
		/// for creating instances of <see cref="IUnitOfWork"/> instances.</param>
		/// <param name="isolationLevel">One of the values of <see cref="IsolationLevel"/> that specifies the transaction
		/// isolation level of the <see cref="UnitOfWorkScopeTransaction"/> instance.</param>
		public UnitOfWorkScopeTransaction(IUnitOfWorkFactory unitOfWorkFactory, IsolationLevel isolationLevel)
		{
			Guard.Against<ArgumentNullException>(unitOfWorkFactory == null,
			                                     "A valid non-null instance that implements the IUnitOfWorkFactory is required.");
			_transactionId = new Guid();
			_transactionRolledback = false;
			_disposed = false;
			_unitOfWork = unitOfWorkFactory.Create();
			_runningTransaction = _unitOfWork.BeginTransaction(isolationLevel);
			_isolationLevel = isolationLevel;
			_attachedScopes = new Stack<UnitOfWorkScope>();
		}

		/// <summary>
		/// Gets a <see cref="Guid"/> that uniqely identifies the transaction.
		/// </summary>
		/// <value>A <see cref="Guid"/> that uniquely identifies the transaction.</value>
		public Guid TransactionID
		{
			get { return _transactionId; }
		}

		/// <summary>
		/// Gets the <see cref="IsolationLevel"/> of the <see cref="UnitOfWorkScopeTransaction"/> instance.
		/// </summary>
		/// <value>One of the values of <see cref="IsolationLevel"/>.</value>
		public IsolationLevel IsolationLevel
		{
			get { return _isolationLevel; }
		}

		/// <summary>
		/// Gets the <see cref="IUnitOfWork"/> instance of the <see cref="UnitOfWorkScopeTransaction"/> instance.
		/// </summary>
		public IUnitOfWork UnitOfWork
		{
			get { return _unitOfWork; }
		}

		/// <summary>
		/// Gets a <see cref="IList{TEntity}"/> containing instances of <see cref="UnitOfWorkScopeTransaction"/> currently
		/// started for the current request / thread.
		/// </summary>
		private static LinkedList<UnitOfWorkScopeTransaction> CurrentTransactions
		{
			get
			{
			    var state = ServiceLocator.Current.GetInstance<IState>();
			    var transactions = state.Local.Get<LinkedList<UnitOfWorkScopeTransaction>>(CurrentTransacitonsKey);
                if (transactions == null)
                {
                    transactions = new LinkedList<UnitOfWorkScopeTransaction>();
                    state.Local.Put(CurrentTransacitonsKey, transactions);
                }
			    return transactions;
			}
		}

		/// <summary>
		/// Gets a <see cref="UnitOfWorkScopeTransaction"/> instance that can be used by a <see cref="UnitOfWorkScope"/> instance.
		/// </summary>
		/// <param name="scope">The <see cref="UnitOfWorkScope"/> instance that is requesting the transaction.</param>
		/// <param name="isolationLevel">One of the values of <see cref="IsolationLevel"/> that specifies the transaction isolation level.</param>
		/// <returns>A <see cref="UnitOfWorkScopeTransaction"/> instance.</returns>
		public static UnitOfWorkScopeTransaction GetTransactionForScope(UnitOfWorkScope scope,
		                                                                IsolationLevel isolationLevel)
		{
			return GetTransactionForScope(scope, isolationLevel, UnitOfWorkScopeTransactionOptions.UseCompatible);
		}

		/// <summary>
		/// Gets a <see cref="UnitOfWorkScopeTransaction"/> instance that can be used by a <see cref="UnitOfWorkScope"/> instance.
		/// </summary>
		/// <param name="scope">The <see cref="UnitOfWorkScope"/> instance that is requesting the transaction.</param>
		/// <param name="isolationLevel">One of the values of <see cref="IsolationLevel"/> that specifies the transaction isolation level.</param>
		/// <param name="options">One of the values of <see cref="UnitOfWorkScopeTransactionOptions"/> that specifies options for using existing
		/// transacitons or creating new ones.</param>
		/// <returns>A <see cref="UnitOfWorkScopeTransaction"/> instance.</returns>
		public static UnitOfWorkScopeTransaction GetTransactionForScope(UnitOfWorkScope scope,
		                                                                IsolationLevel isolationLevel,
		                                                                UnitOfWorkScopeTransactionOptions options)
		{
			var useCompatibleTx = (options & UnitOfWorkScopeTransactionOptions.UseCompatible) ==
			                      UnitOfWorkScopeTransactionOptions.UseCompatible;
			var createNewTx = (options & UnitOfWorkScopeTransactionOptions.CreateNew) ==
			                  UnitOfWorkScopeTransactionOptions.CreateNew;

			Guard.Against<InvalidOperationException>(useCompatibleTx && createNewTx,
			                                         "Cannot start a transaction with both UseCompatible and CreateNew specified " +
			                                         "as a UnitOfWorkScopeTransactionOptions");

			if (options == UnitOfWorkScopeTransactionOptions.UseCompatible)
			{
				var transaction = (from t in CurrentTransactions
				                   where t.IsolationLevel == isolationLevel
				                   select t).FirstOrDefault();
				if (transaction != null)
				{
					transaction.AttachScope(scope);
					return transaction;
				}
			}

			var factory = ServiceLocator.Current.GetInstance<IUnitOfWorkFactory>();
			var newTransaction = new UnitOfWorkScopeTransaction(factory, isolationLevel);
			newTransaction.AttachScope(scope);
			CurrentTransactions.AddFirst(newTransaction);
			return newTransaction;
		}

		/// <summary>
		/// Attaches a <see cref="UnitOfWorkScope"/> instance to the <see cref="UnitOfWorkScopeTransaction"/> instance.
		/// </summary> 
		/// <param name="scope"></param>
		void AttachScope(UnitOfWorkScope scope)
		{
			Guard.Against<ObjectDisposedException>(_disposed,
			                                       "Transaction has been disposed. Cannot attach a scope to a disposed transaction.");
			Guard.Against<ArgumentNullException>(scope == null,
			                                     "Cannot attach a null UnitOfWorkScope instance to the UnitOfWorkScopeTransaction instance.");
			_attachedScopes.Push(scope); //Push the scope on to the top of the stack.
		}

		/// <summary>
		/// Causes a comit operation on the <see cref="UnitOfWorkScopeTransaction"/> instance.
		/// </summary>
		/// <param name="scope">The <see cref="UnitOfWorkScope"/> instance that is calling the commit.</param>
		/// <remarks>
		/// This method can only by called by the scope currently on top of the stack. If Called by another scope then an 
		/// <see cref="InvalidOperationException"/> is called. If the calling scope is last in the attached scope hierarchy,
		/// then a commit is called on the underling unit of work instance.
		/// </remarks>
		public void Commit(UnitOfWorkScope scope)
		{
			Guard.Against<ObjectDisposedException>(_disposed,
			                                       "Transaction has been disposed. Cannot commit a disposed transaction.");
			Guard.Against<InvalidOperationException>(_transactionRolledback,
			                                         "Cannot call commit on a rolledback transaction. A child scope or current scope has already rolled back the transaction. Call Rollback()");
			Guard.Against<ArgumentNullException>(scope == null,
			                                     "Cannot commit the transaction for a null UnitOfWorkScope instance.");
			Guard.Against<InvalidOperationException>(_attachedScopes.Peek() != scope,
			                                         "Commit can only be called by the current UnitOfWorkScope instance. The UnitOfWorkScope provided does not match the current scope on the stack.");

			var currentScope = _attachedScopes.Pop();
			if (_attachedScopes.Count != 0)
				return;

			//The calling UnitOfWorkScope is the root of the transaction.
			try
			{
				_unitOfWork.Flush();
				_runningTransaction.Commit();
				_runningTransaction.Dispose();
				_unitOfWork.Dispose();
				CurrentTransactions.Remove(this);
			}
			catch
			{
				_attachedScopes.Push(currentScope);
				throw;
			}
		}

		/// <summary>
		/// Causes a Rollback operation on the <see cref="UnitOfWorkScopeTransaction"/> instance.
		/// </summary>
		/// <param name="scope">The <see cref="UnitOfWorkScope"/> instance that is calling the commit.</param>
		/// <remarks>
		/// This method can only be called by the scope currently on top of the stack. If called by another scope than the
		/// current <see cref="UnitOfWorkScope"/> instance, then a <see cref="InvalidOperationException"/> is thrown. If the
		/// calling scope is the last in the attached scope hierarchy, then a rollback is called on the underlying UnitOfWork
		/// instance.
		/// </remarks>
		public void Rollback(UnitOfWorkScope scope)
		{
			Guard.Against<ObjectDisposedException>(_disposed,
			                                       "Transaction has been disposed. Cannot rollback a disposed transaction.");
			Guard.Against<ArgumentNullException>(scope == null,
			                                     "Cannot rollback the transaction for a null UnitOfWork instance.");
			Guard.Against<InvalidOperationException>(_attachedScopes.Peek() != scope,
			                                         "Rollback can only be called by the current UnitOfWorkScope instance. The UnitOfWorkScope provided does not match the current scope on the stack.");

			_transactionRolledback = true;

			_attachedScopes.Pop();
			if (_attachedScopes.Count != 0)
				return;

			//The calling UnitOfWorkScope is the root of the transaction.
			_runningTransaction.Rollback();
			_runningTransaction.Dispose();
			_unitOfWork.Dispose();
			CurrentTransactions.Remove(this);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				GC.SuppressFinalize(this);
			}
		}
	}
}