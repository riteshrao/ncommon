using System;
using System.Data;
using System.Data.Objects;
using NCommon.Configuration;

namespace NCommon.Data.EntityFramework
{
    public class EFConfiguration : IDataConfiguration
    {
        readonly EFUnitOfWorkFactory _factory = new EFUnitOfWorkFactory();

        public EFConfiguration WithDefaultIsolation(IsolationLevel isolationLevel)
        {
            _factory.DefaultIsolation = isolationLevel;
            return this;
        }

        public EFConfiguration WithObjectContext(Func<ObjectContext> objectContextProvider)
        {
            _factory.RegisterObjectContextProvider(objectContextProvider);
            return this;
        }

        public void Configure(IContainerAdapter containerAdapter)
        {
            containerAdapter.RegisterInstance<IUnitOfWorkFactory>(_factory);
        }
    }
}