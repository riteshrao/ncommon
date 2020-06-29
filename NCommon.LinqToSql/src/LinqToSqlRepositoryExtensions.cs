using NCommon.ObjectAccess;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NCommon.LinqToSql
{
    public static class LinqToSqlRepositoryExtensions
    {
        public static ILinqToSqlFetchingRepository<TEntity, TRelated> Fetch<TEntity, TRelated>(this IRepository<TEntity> repository, Expression<Func<TEntity, TRelated>> selector) where TEntity : class
        {
            Guard.Against<ArgumentNullException>(repository == null,
                                                 "Expected a non-null IRepository<> instance.");
            var linqToSqlRepository = repository as LinqToSqlRepository<TEntity>;
            Guard.Against<InvalidOperationException>(repository == null,
                 "Cannot use Linq to Sql's FetchMany extension on the underlying repository. The repository " +
                "does not inherit from or is not a LinqToSqlRepository<> instance. The Linq to Sql's fetching extensions can " +
                "only be used by Linq to Sql's repository LinqToSqlRepository<>.");

            linqToSqlRepository.ApplyLoadWith(selector);
            return (ILinqToSqlFetchingRepository<TEntity, TRelated>)
                   Activator.CreateInstance(typeof (LinqToSqlFetchingRepository<TEntity, TRelated>), linqToSqlRepository);
        }

        public static ILinqToSqlFetchingRepository<TEntity, TRelated> FetchMany<TEntity, TRelated>(this IRepository<TEntity> repository, Expression<Func<TEntity, IEnumerable<TRelated>>> selector) where TEntity : class
        {
            Guard.Against<ArgumentNullException>(repository == null,
                                                 "Expected a non-null IRepository<> instance.");
            var linqToSqlRepository = repository as LinqToSqlRepository<TEntity>;
            Guard.Against<InvalidOperationException>(repository == null,
                 "Cannot use Linq to Sql's FetchMany extension on the underlying repository. The repository " +
                "does not inherit or is not a LinqToRepository<> instance. The Linq to Sql's fetching extensions can " +
                "only be used by Linq to Sql's repository LinqToRepository<>.");

            linqToSqlRepository.ApplyLoadWith(selector);
            return (ILinqToSqlFetchingRepository<TEntity, TRelated>)
                   Activator.CreateInstance(typeof(LinqToSqlFetchingRepository<TEntity, TRelated>), linqToSqlRepository);
        }

        public static ILinqToSqlFetchingRepository<TEntity, TRelated> ThenFetch<TEntity, TFetch, TRelated>(
            this ILinqToSqlFetchingRepository<TEntity, TFetch> repository, 
            Expression<Func<TFetch, TRelated>> selector) where TEntity : class
        {
            Guard.Against<ArgumentNullException>(repository == null,
                                                "Expected a non-null ILinqToSqlFetchingRepository<> instance.");
            repository.RootRepository.ApplyLoadWith(selector);
            return (ILinqToSqlFetchingRepository<TEntity, TRelated>)
                  Activator.CreateInstance(typeof(LinqToSqlFetchingRepository<TEntity, TRelated>), repository.RootRepository);
        }

        public static ILinqToSqlFetchingRepository<TEntity, TRelated> ThenFetchMany<TEntity, TFetch, TRelated>(
            this ILinqToSqlFetchingRepository<TEntity, TFetch> repository,
            Expression<Func<TFetch, IEnumerable<TRelated>>> selector) where TEntity : class
        {
            Guard.Against<ArgumentNullException>(repository == null,
                                                "Expected a non-null ILinqToSqlFetchingRepository<> instance.");
            repository.RootRepository.ApplyLoadWith(selector);
            return (ILinqToSqlFetchingRepository<TEntity, TRelated>)
                  Activator.CreateInstance(typeof(LinqToSqlFetchingRepository<TEntity, TRelated>), repository.RootRepository);
        }
    }
}