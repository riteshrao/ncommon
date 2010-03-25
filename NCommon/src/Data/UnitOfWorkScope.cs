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
using NCommon.Data.Impl;
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
        bool _disposed;
        bool _commitAttempted;
        bool _committed;

        /// <summary>
        /// 
        /// </summary>
        public event Action<UnitOfWorkScope> ScopeComitting;

        /// <summary>
        /// 
        /// </summary>
        public event Action<UnitOfWorkScope> ScopeRollingback;

        /// <summary>
        /// Default Constuctor.
        /// Creates a new <see cref="UnitOfWorkScope"/> with the <see cref="System.Data.IsolationLevel.Serializable"/> 
        /// transaction isolation level.
        /// </summary>
        public UnitOfWorkScope() : this(false) { }

        /// <summary>
        /// Overloaded Constructor.
        /// Creates a new instance of the <see cref="UnitOfWorkScope"/> class.
        /// </summary>
        /// <param name="newTransaction">To create a new scope that does not enlist in an existing ambient 
        /// <see cref="UnitOfWorkScope"/> or <see cref="TransactionScope"/>, specify new, otherwise specify false.</param>
        public UnitOfWorkScope(bool newTransaction)
        {
            UnitOfWorkManager.EnlistScope(this, newTransaction);
        }

        ///<summary>
        /// Commits the current running transaction in the scope.
        ///</summary>
        public void Commit()
        {
            Guard.Against<ObjectDisposedException>(_disposed,
                                                   "Cannot commit a disposed UnitOfWorkScope instance.");
            Guard.Against<InvalidOperationException>(_commitAttempted || _committed,
                                                     "The UnitOfWorkScope has already been committed or has attempted to commit and failed. Cannot commit a UnitOfWorkScope");
            _commitAttempted = true;
            OnCommit();
            _committed = true;
        }

        /// <summary>
        /// 
        /// </summary>
        void OnCommit()
        {
            if (ScopeComitting != null)
                ScopeComitting(this);
        }

        /// <summary>
        /// 
        /// </summary>
        void OnRollback()
        {
            if (ScopeRollingback != null)
                ScopeRollingback(this);
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
        void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                try
                {
                    if (!_commitAttempted && !_committed && UnitOfWorkConfiguration.AutoCompleteScope)
                        OnCommit();
                    else
                        OnRollback();
                }
                finally
                {
                    ScopeComitting = null;
                    ScopeRollingback = null;
                    _disposed = true;
                }
            }
        }
    }
}
