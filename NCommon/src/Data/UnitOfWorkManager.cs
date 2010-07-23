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
using Common.Logging;
using Microsoft.Practices.ServiceLocation;
using NCommon.Data.Impl;
using NCommon.State;

namespace NCommon.Data
{
    ///<summary>
    /// Gets an instances of <see cref="ITransactionManager"/>.
    ///</summary>
    public static class UnitOfWorkManager
    {
        const string LocalTransactionManagerKey = "UnitOfWorkManager.LocalTransactionManager";
        static Func<ITransactionManager> _provider;
        static ILog _logger = LogManager.GetLogger(typeof(UnitOfWorkManager));

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="UnitOfWorkManager"/>.
        /// </summary>
        static UnitOfWorkManager()
        {
            _provider = () =>
            {
                _logger.Debug(x => x("Using default UnitOfWorkManager provider to resolve current transaction manager."));
                var state = ServiceLocator.Current.GetInstance<ILocalState>();
                var transactionManager = state.Get<ITransactionManager>(LocalTransactionManagerKey);
                if (transactionManager == null)
                {
                    _logger.Debug(x => x("No valid ITransactionManager found in Local state. Creating a new TransactionManager."));
                    transactionManager = new TransactionManager();
                    state.Put(LocalTransactionManagerKey, transactionManager);
                }
                return transactionManager;
            };
        }

        ///<summary>
        /// Sets a <see cref="Func{T}"/> of <see cref="ITransactionManager"/> that the 
        /// <see cref="UnitOfWorkManager"/> uses to get an instance of <see cref="ITransactionManager"/>
        ///</summary>
        ///<param name="provider"></param>
        public static void SetTransactionManagerProvider(Func<ITransactionManager> provider)
        {
            Guard.Against<ArgumentNullException>(provider == null,
                                                 "Expected a non-null Func<ITransactionManager> instance.");

            _logger.Debug(x => x("Default transaction manager overriden for UnitOfWorkTransactionManager."));
            _provider = provider;
        }

        /// <summary>
        /// Gets the current <see cref="ITransactionManager"/>.
        /// </summary>
        public static ITransactionManager CurrentTransactionManager
        {
            get
            {
                return _provider();
            }
        }

        /// <summary>
        /// Gets the current <see cref="IUnitOfWork"/> instance.
        /// </summary>
        public static IUnitOfWork CurrentUnitOfWork
        {
            get
            {
                return _provider().CurrentUnitOfWork;
            }
        }
    }
}