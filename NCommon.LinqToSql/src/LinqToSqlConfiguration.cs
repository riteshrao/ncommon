using System;
using System.Data;
using System.Data.Linq;
using NCommon.Configuration;

namespace NCommon.Data.LinqToSql
{
    public class LinqToSqlConfiguration : IDataConfiguration
    {
        readonly LinqToSqlUnitOfWorkFactory _factory = new LinqToSqlUnitOfWorkFactory();

        public LinqToSqlConfiguration WithDefaultIsolation(IsolationLevel isolationLevel)
        {
            _factory.DefaultIsolation = isolationLevel;
            return this;
        }

        public LinqToSqlConfiguration WithDataContext(Func<DataContext> contextProvider)
        {
            _factory.RegisterDataContextProvider(contextProvider);
            return this;
        }

        public void Configure(IContainerAdapter containerAdapter)
        {
            containerAdapter.RegisterInstance<IUnitOfWorkFactory>(_factory);
        }
    }
}