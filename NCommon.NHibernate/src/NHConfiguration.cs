using System;
using System.Data;
using NCommon.Configuration;
using NHibernate;

namespace NCommon.Data.NHibernate
{
    /// <summary>
    /// Implementation of <see cref="IDataConfiguration"/> that configures NCommon to use NHibernate.
    /// </summary>
    public class NHConfiguration : IDataConfiguration
    {
        readonly NHUnitOfWorkFactory _factory = new NHUnitOfWorkFactory();

        /// <summary>
        /// Registers a <see cref="Func{T}"/> of type <see cref="ISessionFactory"/> provider that can be
        /// used to get instances of <see cref="ISessionFactory"/>.
        /// </summary>
        /// <param name="factoryProvider">An instance of <see cref="Func{T}"/> of type <see cref="ISessionFactory"/></param>
        /// <returns><see cref="NHConfiguration"/></returns>
        public NHConfiguration WithSessionFactory(Func<ISessionFactory> factoryProvider)
        {
            Guard.Against<ArgumentNullException>(factoryProvider == null,
                                                 "Expected a non-null Func<ISessionFactory> instance.");
            _factory.RegisterSessionFactoryProvider(factoryProvider);
            return this;
        }

        /// <summary>
        /// Called by NCommon <see cref="Configure"/> to configure data providers.
        /// </summary>
        /// <param name="containerAdapter">The <see cref="IContainerAdapter"/> instance that allows
        /// registering components.</param>
        public void Configure(IContainerAdapter containerAdapter)
        {
            containerAdapter.RegisterInstance<IUnitOfWorkFactory>(_factory);
            containerAdapter.RegisterGeneric(typeof(IRepository<>), typeof(NHRepository<>));
        }
    }
}