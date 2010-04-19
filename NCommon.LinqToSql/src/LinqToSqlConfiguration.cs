using System;
using System.Data;
using System.Data.Linq;
using NCommon.Configuration;

namespace NCommon.Data.LinqToSql
{
    /// <summary>
    /// Implementatio of <see cref="IDataConfiguration"/> that configured NCommon to use Linq To Sql
    /// </summary>
    public class LinqToSqlConfiguration : IDataConfiguration
    {
        readonly LinqToSqlUnitOfWorkFactory _factory = new LinqToSqlUnitOfWorkFactory();

        /// <summary>
        /// Registers a <see cref="DataContext"/> provider.
        /// </summary>
        /// <param name="contextProvider">A <see cref="Func{T}"/> of type <see cref="DataContext"/>.</param>
        /// <returns><see cref="LinqToSqlConfiguration"/></returns>
        public LinqToSqlConfiguration WithDataContext(Func<DataContext> contextProvider)
        {
            _factory.RegisterDataContextProvider(contextProvider);
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
            containerAdapter.RegisterGeneric(typeof(IRepository<>), typeof(LinqToSqlRepository<>));
        }
    }
}