using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NCommon.Data.Language
{
    /// <summary>
    /// Defines the root interface to specify eager fetching strategy for a <see cref="IRepository{TEntity}"/>
    /// </summary>
    /// <typeparam name="T">The entity for eager fetching strategy.</typeparam>
    public class RepositoryEagerFetchingStrategy<T>
    {
        IList<Expression> _paths = new List<Expression>();

        public Expression[] Paths
        {
            get { return _paths.ToArray();}
        }

        public EagerFetchingPath<TChild> Fetch<TChild>(Expression<Func<T, object>> path)
        {
            _paths.Add(path);
            return new EagerFetchingPath<TChild>(_paths);
        }
    }
}