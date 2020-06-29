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
using NCommon.DependencyInjection;
using NCommon.StateStorage;

namespace NCommon.DataServices.Transactions
{
    ///<summary>
    /// Gets an instances of <see cref="IUnitOfWorkTransactionManager"/>.
    ///</summary>
    public static class UnitOfWorkManager
    {
        static Func<IUnitOfWorkTransactionManager> _provider;
        static readonly ILog Logger = LogManager.GetLogger(typeof(UnitOfWorkManager));
        private const string LocalTransactionManagerKey = "UnitOfWorkManager.LocalTransactionManager";
        static readonly Func<IUnitOfWorkTransactionManager> DefaultTransactionManager = () =>
        {
            Logger.Debug(x => x("Using default UnitOfWorkManager provider to resolve current transaction manager."));
            var state = ServiceLocatorWorker.GetInstance<IState>();
            var transactionManager = state.Local.Get<IUnitOfWorkTransactionManager>(LocalTransactionManagerKey);
            if (transactionManager == null)
            {
                Logger.Debug(x => x("No valid ITransactionManager found in Local state. Creating a new TransactionManager."));
                transactionManager = new UnitOfWorkTransactionManager();
                state.Local.Put(LocalTransactionManagerKey, transactionManager);
            }
            return transactionManager;
        };

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="UnitOfWorkManager"/>.
        /// </summary>
        static UnitOfWorkManager()
        {
            _provider = DefaultTransactionManager;
        }

        ///<summary>
        /// Sets a <see cref="Func{T}"/> of <see cref="IUnitOfWorkTransactionManager"/> that the 
        /// <see cref="UnitOfWorkManager"/> uses to get an instance of <see cref="IUnitOfWorkTransactionManager"/>
        ///</summary>
        ///<param name="provider"></param>
        public static void SetTransactionManagerProvider(Func<IUnitOfWorkTransactionManager> provider)
        {
            if (provider == null)
            {
                Logger.Debug(x => x("The transaction manager provide is being set to null. Using " +
                                    " the transaction manager to the default transaction manager provider."));
                _provider = DefaultTransactionManager;
                return;
            }
            Logger.Debug(x => x("The transaction manager provider is being overriden. Using supplied" +
                                " trasaction manager provider."));
            _provider = provider;
        }

        /// <summary>
        /// Gets the current <see cref="IUnitOfWorkTransactionManager"/>.
        /// </summary>
        public static IUnitOfWorkTransactionManager CurrentTransactionManager
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