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

        ///<summary>
        /// An array of <see cref="Expression"/> containing the eager fetching paths.
        ///</summary>
        public IEnumerable<Expression> Paths
        {
            get { return _paths.ToArray();}
        }

        ///<summary>
        /// Specify the path to eagerly fetch.
        ///</summary>
        ///<param name="path"></param>
        ///<typeparam name="TChild"></typeparam>
        ///<returns></returns>
        public EagerFetchingPath<TChild> Fetch<TChild>(Expression<Func<T, object>> path)
        {
            _paths.Add(path);
            return new EagerFetchingPath<TChild>(_paths);
        }
    }
}