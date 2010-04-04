using System.Transactions;
using NCommon.Data;
using NCommon.Data.Impl;
using TransactionManager = NCommon.Data.Impl.TransactionManager;

namespace NCommon.Configuration
{
    ///<summary>
    /// Implementation of <see cref="IUnitOfWorkConfiguration"/>.
    ///</summary>
    public class DefaultUnitOfWorkConfiguration : IUnitOfWorkConfiguration
    {
        /// <summary>
        /// Configures <see cref="UnitOfWorkScope"/> settings.
        /// </summary>
        /// <param name="containerAdapter">The <see cref="IContainerAdapter"/> instance.</param>
        public void Configure(IContainerAdapter containerAdapter)
        {
            containerAdapter.Register<ITransactionManager, TransactionManager>();
        }

        /// <summary>
        /// Gets or sets a boolean value indicating weather <see cref="UnitOfWorkScope"/>
        /// instances should auto-complete the scope when disposing.
        /// </summary>
        public bool AutoCompleteScope
        {
            get { return UnitOfWorkSettings.AutoCompleteScope; }
            set { UnitOfWorkSettings.AutoCompleteScope = value; }
        }

        /// <summary>
        /// Sets the default <see cref="IsolationLevel"/> that <see cref="UnitOfWorkScope"/> instances.
        /// </summary>
        public IsolationLevel DefaultIsolation
        {
            get { return UnitOfWorkSettings.DefaultIsolation; }
            set { UnitOfWorkSettings.DefaultIsolation = value; }
        }
    }
}