using System;
using System.Linq.Expressions;

namespace NCommon.Data.Language
{
    ///<summary>
    /// Represents an eagerly fetched path.
    ///</summary>
    ///<typeparam name="T">The entity type being eagerly fetched.</typeparam>
    public interface IEagerFetchingPath<T>
    {
        ///<summary>
        /// Specify an eager fetching path on <typeparamref name="T"/>.
        ///</summary>
        ///<param name="path"></param>
        ///<typeparam name="TChild"></typeparam>
        ///<returns>The eagerly fetched path.</returns>
        IEagerFetchingPath<TChild> And<TChild>(Expression<Func<T, object>> path);
    }
}