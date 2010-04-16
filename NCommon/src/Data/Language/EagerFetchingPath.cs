using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NCommon.Data.Language
{
    ///<summary>
    ///</summary>
    ///<typeparam name="T"></typeparam>
    public class EagerFetchingPath<T> : IEagerFetchingPath<T>
    {
        readonly IList<Expression> _paths;

        ///<summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="EagerFetchingPath{T}"/> instance.
        ///</summary>
        ///<param name="paths"></param>
        public EagerFetchingPath(IList<Expression> paths)
        {
            _paths = paths;
        }

        ///<summary>
        /// Specify an eager fetching path on <typeparamref name="T"/>.
        ///</summary>
        ///<param name="path"></param>
        ///<typeparam name="TChild"></typeparam>
        ///<returns>The eagerly fetched path.</returns>
        public IEagerFetchingPath<TChild> And<TChild>(Expression<Func<T, object>> path)
        {
            _paths.Add(path);
            return new EagerFetchingPath<TChild>(_paths);
        }
    }
}