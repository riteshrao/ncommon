using System.Transactions;
using NCommon.Data;

namespace NCommon.Configuration
{
    /// <summary>
    /// Configuration settings for <see cref="UnitOfWorkScope"/> instances in NCommon.
    /// </summary>
    public interface IUnitOfWorkConfiguration
    {
        /// <summary>
        /// Sets the default <see cref="IsolationLevel"/> that <see cref="UnitOfWorkScope"/> instances.
        /// </summary>
        IsolationLevel DefaultIsolation { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating weather <see cref="UnitOfWorkScope"/>
        /// instances should auto-complete the scope when disposing.
        /// </summary>
        bool AutoCompleteScope { get; set; }

        /// <summary>
        /// Configures <see cref="UnitOfWorkScope"/> settings.
        /// </summary>
        /// <param name="containerAdapter">The <see cref="IContainerAdapter"/> instance.</param>
        void Configure(IContainerAdapter containerAdapter);
    }
}