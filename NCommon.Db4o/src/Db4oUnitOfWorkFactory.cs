using System;
using Db4objects.Db4o;
using NCommon.Data;

namespace NCommon.Db4o
{
    /// <summary>
    /// <see cref="IUnitOfWorkFactory"/> implementation for <see cref="Db4oUnitOfWork"/> instances.
    /// </summary>
    public class Db4oUnitOfWorkFactory : IUnitOfWorkFactory
    {
        static Func<IObjectContainer> _containerProvider;
        static readonly object _setContainerProviderLock = new object();

        public static void SetContainerProvider(Func<IObjectContainer> containerProvider)
        {
            lock(_setContainerProviderLock)
                _containerProvider = containerProvider;       
        }

        public IUnitOfWork Create()
        {
            Guard.Against<InvalidOperationException>(_containerProvider == null,
                                                    "A IObjectContainer provider has not been specified. Please specify a " +
                                                     "provider using SetContainerProvider before creating Db4oUnitOfWork instances");
            return new Db4oUnitOfWork(_containerProvider());
        }
    }
}