using System;
using System.Data;
using NCommon.Configuration;
using NHibernate;

namespace NCommon.Data.NHibernate
{
    public class NHConfiguration : IDataConfiguration
    {
        readonly NHUnitOfWorkFactory _factory = new NHUnitOfWorkFactory();

        public NHConfiguration WithDefaultIsolation(IsolationLevel isolationLevel)
        {
            _factory.DefaultIsolation = isolationLevel;
            return this;
        }

        public NHConfiguration WithSessionFactory(Func<ISessionFactory> factoryProvider)
        {
            _factory.RegisterSessionFactoryProvider(factoryProvider);
            return this;
        }

        public void Configure(IContainerAdapter containerAdapter)
        {
            containerAdapter.RegisterInstance<IUnitOfWorkFactory>(_factory);
        }
    }
}