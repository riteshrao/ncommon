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
        bool _autoCompleteScope = false;
        IsolationLevel _defaultIsolation = IsolationLevel.ReadCommitted;

        /// <summary>
        /// Configures <see cref="UnitOfWorkScope"/> settings.
        /// </summary>
        /// <param name="containerAdapter">The <see cref="IContainerAdapter"/> instance.</param>
        public void Configure(IContainerAdapter containerAdapter)
        {
            containerAdapter.Register<ITransactionManager, TransactionManager>();
            UnitOfWorkSettings.AutoCompleteScope = _autoCompleteScope;
            UnitOfWorkSettings.DefaultIsolation = _defaultIsolation;
        }

        /// <summary>
        /// Sets <see cref="UnitOfWorkScope"/> instances to auto complete when disposed.
        /// </summary>
        public IUnitOfWorkConfiguration AutoCompleteScope()
        {
            _autoCompleteScope = true;
            return this;
        }

        /// <summary>
        /// Sets the default isolation level used by <see cref="UnitOfWorkScope"/>.
        /// </summary>
        /// <param name="isolationLevel"></param>
        public IUnitOfWorkConfiguration WithDefaultIsolation(IsolationLevel isolationLevel)
        {
            _defaultIsolation = isolationLevel;
            return this;
        }
    }
}