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
using System.Transactions;
using Microsoft.Practices.ServiceLocation;
using NCommon.State;
using IsolationLevel=System.Data.IsolationLevel;

namespace NCommon.Data
{
    /// <summary>
    /// Helper class that allows starting and using a unit of work like:
    /// <![CDATA[
    ///     using (UnitOfWorkScope scope = new UnitOfWorkScope()) {
    ///         //Do some stuff here.
    ///         scope.Commit();
    ///     }
    /// 
    /// ]]>
    /// </summary>
    public class UnitOfWorkScope : IDisposable
    {
        #region fields

        private static readonly string UnitOfWorkScopeStackKey = typeof (UnitOfWorkScope).FullName +
                                                                 ".RunningScopeStack";

        private UnitOfWorkScopeTransaction _currentTransaction;
        private bool _disposed;
    	private readonly bool _autoComplete;
        #endregion

        #region ctor

        /// <summary>
        /// Default Constuctor.
        /// Creates a new <see cref="UnitOfWorkScope"/> with the <see cref="System.Data.IsolationLevel.Serializable"/> 
        /// transaction isolation level.
        /// </summary>
        public UnitOfWorkScope() : this(GetScopeIsolationLevel(), UnitOfWorkScopeTransactionOptions.UseCompatible)
        {
        }

		/// <summary>
		/// Overloaded Constructor.
		/// Creates a new instance of the <see cref="UnitOfWorkScope"/> with the 
		/// specified <see cref="UnitOfWorkScopeTransactionOptions"/> and default <see cref="IsolationLevel"/>
		/// </summary>
		/// <param name="options"></param>
		public UnitOfWorkScope(UnitOfWorkScopeTransactionOptions options) : this(GetScopeIsolationLevel(), options)
		{
		}


    	/// <summary>
        /// Overloaded Constructor.
        /// Creates a new instance of <see cref="UnitOfWorkScope"/> with the specified transaction
        /// isolation level.
        /// </summary>
        /// <param name="isolationLevel">One of the values of <see cref="System.Data.IsolationLevel"/> that specifies
        /// the transation isolation level the scope should use.</param>
        public UnitOfWorkScope(IsolationLevel isolationLevel)
            : this(isolationLevel, UnitOfWorkScopeTransactionOptions.UseCompatible)
        {
        }

        /// <summary>
        /// Overloaded Constructor.
        /// Creates a new instance of <see cref="UnitOfWorkScope"/> with the specified transaction isolation level, option connection and
        /// a transaction option that specifies if an existing transaction should be used or to create a new transaction.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <param name="transactionOptions"></param>
        public UnitOfWorkScope(IsolationLevel isolationLevel, UnitOfWorkScopeTransactionOptions transactionOptions)
        {
            _disposed = false;
        	_autoComplete = (transactionOptions & UnitOfWorkScopeTransactionOptions.AutoComplete) ==
        	                UnitOfWorkScopeTransactionOptions.AutoComplete;
            _currentTransaction = UnitOfWorkScopeTransaction.GetTransactionForScope(this, isolationLevel,
                                                                                    transactionOptions);
            RegisterScope(this);
        }

        #endregion

        #region properties

        /// <summary>
        /// Checks if the current thread or request has a <see cref="UnitOfWorkScope"/> instance started.
        /// </summary>
        /// <value>True if a <see cref="UnitOfWorkScope"/> instance has started and is present.</value>
        public static bool HasStarted
        {
            get
            {
                var state = ServiceLocator.Current.GetInstance<IState>();
                var unitOfWorkScopes = state.Local.Get<Stack<UnitOfWorkScope>>(UnitOfWorkScopeStackKey);
                if (unitOfWorkScopes == null)
                    return false;
                return unitOfWorkScopes.Count > 0;
            }
        }

        /// <summary>
        /// Gets the current <see cref="UnitOfWorkScope"/> instance for the current thread or request.
        /// </summary>
        /// <value>The current and most recent <see cref="UnitOfWorkScope"/> instance started for the current thread or request.
        /// If none started, then a null reference is returned.</value>
        public static UnitOfWorkScope Current
        {
            get
            {
                if (RunningScopes.Count == 0)
                    return null;
                return RunningScopes.Peek();
            }
        }

        /// <summary>
        /// Gets a <see cref="Stack{TEntity}"/> of <see cref="UnitOfWorkScope"/> that is used to store and retrieve
        /// running scope instances.
        /// </summary>
        private static Stack<UnitOfWorkScope> RunningScopes
        {
            get
            {
                //Note: No locking is required since the stack is stored either on the current thread or on the current request. 
                var state = ServiceLocator.Current.GetInstance<IState>();
                var unitOfWorkScopes = state.Local.Get<Stack<UnitOfWorkScope>>(UnitOfWorkScopeStackKey);
                if (unitOfWorkScopes == null)
                {
                    unitOfWorkScopes = new Stack<UnitOfWorkScope>();
                    state.Local.Put<Stack<UnitOfWorkScope>>(UnitOfWorkScopeStackKey, unitOfWorkScopes);
                }
                return unitOfWorkScopes;
            }
        }

        /// <summary>
        /// Gets the <see cref="UnitOfWorkScope"/> instance used by the <see cref="IUnitOfWork"/> instance.
        /// </summary>
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _currentTransaction.UnitOfWork;
            }
        }

        #endregion

        #region methods
		/// <summary>
		/// Gets the current isolation level to use for a <see cref="UnitOfWorkScopeTransaction"/>
		/// </summary>
		/// <returns></returns>
		private static IsolationLevel GetScopeIsolationLevel()
		{
			return Transaction.Current == null ? IsolationLevel.ReadCommitted : MapToSystemDataIsolationLevel(Transaction.Current.IsolationLevel);
		}

		/// <summary>
		/// Maps a <see cref="System.Transactions.IsolationLevel"/> to <see cref="IsolationLevel"/>
		/// </summary>
		/// <param name="isolationLevel">The isolation level to map.</param>
		/// <returns>A corresponding <see cref="IsolationLevel"/></returns>
    	private static IsolationLevel MapToSystemDataIsolationLevel(System.Transactions.IsolationLevel isolationLevel)
    	{
			switch (isolationLevel)
			{
				case System.Transactions.IsolationLevel.Chaos:
					return IsolationLevel.Chaos;
				case System.Transactions.IsolationLevel.ReadCommitted:
					return IsolationLevel.ReadCommitted;
				case System.Transactions.IsolationLevel.ReadUncommitted:
					return IsolationLevel.ReadUncommitted;
				case System.Transactions.IsolationLevel.RepeatableRead:
					return IsolationLevel.RepeatableRead;
				case System.Transactions.IsolationLevel.Serializable:
					return IsolationLevel.Serializable;
				case System.Transactions.IsolationLevel.Snapshot:
					return IsolationLevel.Snapshot;
				case System.Transactions.IsolationLevel.Unspecified:
					return IsolationLevel.Unspecified;
				default:
					return IsolationLevel.ReadCommitted; //default;
			}
    	}

    	/// <summary>
        /// Disposes off the <see cref="UnitOfWorkScope"/> insance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes off the managed and un-managed resources used.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_disposed) return;

            if (_currentTransaction != null)
            {
				if (_autoComplete)
				{
					try
					{
						_currentTransaction.Commit(this);
					}
					catch
					{
						_currentTransaction.Rollback(this);
						//Continue disposing
						_currentTransaction = null;
						UnRegisterScope(this);
						_disposed = true;

						//throw;
						throw;
					}
				}
				else
					_currentTransaction.Rollback(this);
                _currentTransaction = null;
            }
            UnRegisterScope(this);
            _disposed = true;
        }

        /// <summary>
        /// Registers a scope as the top level scope on the <see cref="RunningScopes"/> stack.
        /// </summary>
        /// <param name="scope">The <see cref="UnitOfWorkScope"/> instance to set as the top level scope on the stack.</param>
        private static void RegisterScope(UnitOfWorkScope scope)
        {
            Guard.Against<ArgumentNullException>(scope == null,
                                                 "Cannot register a null UnitOfWorkScope instance as the top level scope.");
            Data.UnitOfWork.Current = scope.UnitOfWork;
            //Setting the UnitOfWork isntance held by the scope as the current scope.
            RunningScopes.Push(scope);
        }

        /// <summary>
        /// UnRegisters a <see cref="UnitOfWorkScope"/> as the top level scope on the stack.
        /// </summary>
        /// <param name="scope"></param>
        private static void UnRegisterScope(UnitOfWorkScope scope)
        {
            Guard.Against<ArgumentNullException>(scope == null,
                                                 "Cannot Un-Register a null UnitOfWorkScope instance as the top level scope.");
            Guard.Against<InvalidOperationException>(RunningScopes.Peek() != scope,
                                                     "The UnitOfWorkScope provided does not match the current top level scope. Cannot un-register the specified scope.");
            RunningScopes.Pop();

            if (RunningScopes.Count > 0)
            {
                //If the Stack has additional scopes, set the current unit of work to the UnitOfWork instance held by the top most scope.
                UnitOfWorkScope currentScope = RunningScopes.Peek();
                Data.UnitOfWork.Current = currentScope.UnitOfWork;
            }
            else
                Data.UnitOfWork.Current = null;
        }

        ///<summary>
        /// Commits the current running transaction in the scope.
        ///</summary>
        public void Commit()
        {
            Guard.Against<ObjectDisposedException>(_disposed, "Cannot commit a disposed UnitOfWorkScope instance.");
            _currentTransaction.Commit(this);
            _currentTransaction = null;
        }
        #endregion
    }
}