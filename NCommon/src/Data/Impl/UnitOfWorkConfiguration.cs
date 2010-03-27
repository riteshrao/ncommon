using System;
using System.Transactions;
using NCommon.Configuration;

namespace NCommon.Data.Impl
{
    ///<summary>
    /// Implementation of <see cref="IUnitOfWorkConfiguration"/>.
    ///</summary>
    public class UnitOfWorkConfiguration : IUnitOfWorkConfiguration
    {
        /// <summary>
        /// Gets the default <see cref="IsolationLevel"/>.
        /// </summary>
        public static IsolationLevel DefaultIsolation { get;  set; }

        /// <summary>
        /// Gets a boolean value indicating weather to auto complete
        /// <see cref="UnitOfWorkScope"/> instances.
        /// </summary>
        public static bool AutoCompleteScope { get;  set; }

        /// <summary>
        /// Configures <see cref="UnitOfWorkScope"/> settings.
        /// </summary>
        /// <param name="containerAdapter">The <see cref="IContainerAdapter"/> instance.</param>
        public void Configure(IContainerAdapter containerAdapter)
        {
            containerAdapter.Register<ITransactionManager, TransactionManager>();
        }

        bool IUnitOfWorkConfiguration.AutoCompleteScope
        {
            get { return AutoCompleteScope; }
            set { AutoCompleteScope = value; }
        }

        IsolationLevel IUnitOfWorkConfiguration.DefaultIsolation
        {
            get { return DefaultIsolation; }
            set { DefaultIsolation = value; }
        }
    }
}