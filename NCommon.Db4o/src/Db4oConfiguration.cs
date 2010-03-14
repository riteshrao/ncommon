using System;
using Db4objects.Db4o;
using NCommon.Configuration;

namespace NCommon.Data.Db4o
{
    /// <summary>
    /// Implementation of <see cref="IDataConfiguration"/> for Db4o.
    /// </summary>
    public class Db4oConfiguration : IDataConfiguration
    {
        Func<IObjectContainer> _containerProvider;

        /// <summary>
        /// Registers a <see cref="Func{T}"/> of <see cref="IObjectContainer"/> type that
        /// can be used to create <see cref="IObjectContainer"/> instances.
        /// </summary>
        /// <param name="containerProvider">A <see cref="Func{T}"/> of type <see cref="IObjectContainer"/>
        /// instance.</param>
        /// <returns><see cref="Db4oConfiguration"/></returns>
        public Db4oConfiguration WithContainer(Func<IObjectContainer> containerProvider)
        {
            Guard.Against<ArgumentNullException>(containerProvider == null,
                                                 "Expected a non-null Func<IObjectContainer> instance.");
            _containerProvider = containerProvider;
            return this;
        }

        /// <summary>
        /// Called by NCommon <see cref="Configure"/> to configure data providers.
        /// </summary>
        /// <param name="containerAdapter">The <see cref="IContainerAdapter"/> instance that allows
        /// registering components.</param>
        public void Configure(IContainerAdapter containerAdapter)
        {
            containerAdapter.Register<IUnitOfWorkFactory, Db4oUnitOfWorkFactory>();
            containerAdapter.Register(typeof (IRepository<>), typeof (Db4oRepository<>));
            Db4oUnitOfWorkFactory.SetContainerProvider(_containerProvider);
        }
    }
}