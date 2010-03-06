using System;
using Db4objects.Db4o;
using NCommon.Configuration;

namespace NCommon.Data.Db4o
{
    public class Db4oConfiguration : IDataConfiguration
    {
        Func<IObjectContainer> _containerProvider;

        public Db4oConfiguration WithContainer(Func<IObjectContainer> container)
        {
            _containerProvider = container;
            return this;
        }

        public void Configure(IContainer container)
        {
            container.Register<IUnitOfWorkFactory, Db4oUnitOfWorkFactory>();
            container.Register(typeof (IRepository<>), typeof (Db4oRepository<>));
            Db4oUnitOfWorkFactory.SetContainerProvider(_containerProvider);
        }
    }
}