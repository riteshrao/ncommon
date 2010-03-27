using System;
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

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="UnitOfWorkManager"/>.
        /// </summary>
        static UnitOfWorkManager()
        {
            _provider = () =>
            {
                var state = ServiceLocator.Current.GetInstance<IState>();
                var transactionManager = state.Local.Get<ITransactionManager>(LocalTransactionManagerKey);
                if (transactionManager == null)
                {
                    transactionManager = new TransactionManager();
                    state.Local.Put(LocalTransactionManagerKey, transactionManager);
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