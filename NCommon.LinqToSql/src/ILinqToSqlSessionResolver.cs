using System;
using System.Data.Linq;

namespace NCommon.Data.LinqToSql
{
    /// <summary>
    /// Interface implemented by a custom resolver for Entity Framework that resolves <see cref="DataContext"/>
    /// instances for a type.
    /// </summary>
    public interface ILinqToSqlSessionResolver
    {
        /// <summary>
        /// Gets the unique <see cref="ILinqToSqlSession"/> key for a type. 
        /// </summary>
        /// <typeparam name="T">The type for which the ObjectContext key should be retrieved.</typeparam>
        /// <returns>A <see cref="Guid"/> representing the unique object context key.</returns>
        Guid GetSessionKeyFor<T>();
        /// <summary>
        /// Gets a <see cref="ILinqToSqlSession"/> instance for a given type.
        /// </summary>
        /// <typeparam name="T">The type for which an <see cref="ILinqToSqlSession"/> is returned.</typeparam>
        /// <returns>An instance of <see cref="ILinqToSqlSession"/>.</returns>
        ILinqToSqlSession OpenSessionFor<T>();
        /// <summary>
        /// Gets the <see cref="DataContext"/> that can be used to query and update a given type.
        /// </summary>
        /// <typeparam name="T">The type for which an <see cref="DataContext"/> is returned.</typeparam>
        /// <returns>An <see cref="DataContext"/> that can be used to query and update the given type.</returns>
        DataContext GetDataContextFor<T>();
        /// <summary>
        /// Registers an <see cref="DataContext"/> provider with the resolver.
        /// </summary>
        /// <param name="contextProvider">A <see cref="Func{T}"/> of type <see cref="DataContext"/>.</param>
        void RegisterDataContextProvider(Func<DataContext> contextProvider);
        /// <summary>
        /// Gets the count of <see cref="DataContext"/> providers registered with the resolver.
        /// </summary>
        int DataContextsRegistered { get; }
    }
}