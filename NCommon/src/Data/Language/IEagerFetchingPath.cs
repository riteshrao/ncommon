using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NCommon.Data.Language
{
    public interface IEagerFetchingPath<T>
    {
        IEagerFetchingPath<TChild> And<TChild>(Expression<Func<T, object>> path);
    }

    public class EagerFetchingPath<T> : IEagerFetchingPath<T>
    {
        IList<Expression> _paths;

        public EagerFetchingPath(IList<Expression> paths)
        {
            _paths = paths;
        }

        public IEagerFetchingPath<TChild> And<TChild>(Expression<Func<T, object>> path)
        {
            _paths.Add(path);
            return new EagerFetchingPath<TChild>(_paths);
        }
    }
}