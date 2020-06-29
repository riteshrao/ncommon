using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NCommon.Expressions;
using NCommon.ObjectAccess;

namespace NCommon.Data.EntityFramework
{
    public static class EFRepositoryExtensions
    {
        public static IEFFetchingRepository<TEntity, TReleated> Fetch<TEntity, TReleated>(this IRepository<TEntity> repository, Expression<Func<TEntity, TReleated>> selector) where TEntity : class
        {
            Guard.Against<ArgumentNullException>(repository == null, "Expected a non-null IRepository<> instance.");

            var efRepository = repository as EFRepository<TEntity>;
            Guard.Against<InvalidOperationException>(efRepository == null,
                "Cannot use Entity Framework's Fetch extension on the underlying repository. The repository " +
                "does not inherit or is not a EFRepository<> instance. The Entity Framework's fetching extensions can " +
                "only be used by entity framework's repository EFRepository<>.");

            var visitor = new MemberAccessPathVisitor();
            visitor.Visit(selector);
            efRepository.AddInclude(visitor.Path);

            return (IEFFetchingRepository<TEntity, TReleated>)
                Activator.CreateInstance(typeof(EFFetchingRepository<TEntity, TReleated>), efRepository, visitor.Path);
        }

        public static IEFFetchingRepository<TEntity, TReleated> FetchMany<TEntity, TReleated>(this IRepository<TEntity> repository, Expression<Func<TEntity, IEnumerable<TReleated>>> selector) where TEntity : class
        {
            Guard.Against<ArgumentNullException>(repository == null, "Expected a non-null IRepository<> instance.");

            var efRepository = repository as EFRepository<TEntity>;
            Guard.Against<InvalidOperationException>(efRepository == null,
                "Cannot use Entity Framework's FetchMany extension on the underlying repository. The repository " +
                "does not inherit or is not a EFRepository<> instance. The Entity Framework's fetching extensions can " +
                "only be used by entity framework's repository EFRepository<>.");

            var visitor = new MemberAccessPathVisitor();
            visitor.Visit(selector);
            efRepository.AddInclude(visitor.Path);

            return (IEFFetchingRepository<TEntity, TReleated>)
                Activator.CreateInstance(typeof(EFFetchingRepository<TEntity, TReleated>), efRepository, visitor.Path);
        }

        public static IEFFetchingRepository<TEntity, TReleated> ThenFetch<TEntity, TFetch, TReleated>(this IEFFetchingRepository<TEntity, TFetch> repository, Expression<Func<TFetch, TReleated>> selector) where TEntity : class
        {
            Guard.Against<ArgumentNullException>(repository == null, "Expected a non-null IEFFetchingRepository<> instance.");

            var visitor = new MemberAccessPathVisitor();
            visitor.Visit(selector);
            var includePath = repository.FetchingPath + "." + visitor.Path;
            repository.RootRepository.AddInclude(includePath);

            return (IEFFetchingRepository<TEntity, TReleated>)
                Activator.CreateInstance(typeof(EFFetchingRepository<TEntity, TReleated>), repository.RootRepository, includePath);
        }

        public static IEFFetchingRepository<TEntity, TReleated> ThenFetchMany<TEntity, TFetch, TReleated>(this IEFFetchingRepository<TEntity, TFetch> repository, Expression<Func<TFetch, IEnumerable<TReleated>>> selector) where TEntity : class
        {
            Guard.Against<ArgumentNullException>(repository == null, "Expected a non-null IEFFetchingRepository<> instance.");

            var visitor = new MemberAccessPathVisitor();
            visitor.Visit(selector);
            var includePath = repository.FetchingPath + "." + visitor.Path;
            repository.RootRepository.AddInclude(includePath);

            return (IEFFetchingRepository<TEntity, TReleated>)
                Activator.CreateInstance(typeof(EFFetchingRepository<TEntity, TReleated>), repository.RootRepository, includePath);
        }
    }
}