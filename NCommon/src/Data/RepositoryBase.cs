#region license
//Copyright 2008 Ritesh Rao 

//Licensed under the Apache License, Version 2.0 (the "License"); 
//you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 

//http://www.apache.org/licenses/LICENSE-2.0 

//Unless required by applicable law or agreed to in writing, software 
//distributed under the License is distributed on an "AS IS" BASIS, 
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and 
//limitations under the License. 
#endregion


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.ServiceLocation;
using NCommon.Extensions;
using NCommon.Specifications;

namespace NCommon.Data
{
    ///<summary>
    /// A base class for implementors of <see cref="IRepository{TEntity}"/>.
    ///</summary>
    ///<typeparam name="TEntity"></typeparam>
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity>
    {
        #region properties

        /// <summary>
        /// Gets the <see cref="IQueryable{TEntity}"/> used by the <see cref="RepositoryBase{TEntity}"/> 
        /// to execute Linq queries.
        /// </summary>
        /// <value>A <see cref="IQueryable{TEntity}"/> instance.</value>
        /// <remarks>
        /// Inheritos of this base class should return a valid non-null <see cref="IQueryable{TEntity}"/> instance.
        /// </remarks>
        protected abstract IQueryable<TEntity> RepositoryQuery { get; }

        #endregion

        #region methods
        /// <summary>
        /// Gets the <see cref="IUnitOfWork"/> that the repository should use.
        /// </summary>
        /// <typeparam name="TUnitOfWork">A compatible unit of work instance.</typeparam>
        /// <returns></returns>
        protected virtual TUnitOfWork GetCurrentUnitOfWork<TUnitOfWork> () where TUnitOfWork : IUnitOfWork
        {
            Guard.Against<InvalidOperationException>(!UnitOfWork.HasStarted,
                                                    "No compatible UnitOfWork instance was found. Please start a compatible unit of work " +
                                                    "before creating the repository or use the constructor overload to explicitly provide a ObjectContext.");
            Guard.TypeOf<TUnitOfWork>(UnitOfWork.Current,
                                              "The current UnitOfWork instance is not compatible with the repository. " +
                                              "Please start a compatible unit of work before using the repository.");
            return ((TUnitOfWork)UnitOfWork.Current);
        }
        #endregion

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{TEntity}" /> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<TEntity> GetEnumerator()
        {
            return RepositoryQuery.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return RepositoryQuery.GetEnumerator();
        }

        #endregion

        #region Implementation of IQueryable

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="IQueryable" />.
        /// </summary>
        /// <returns>
        /// The <see cref="Expression" /> that is associated with this instance of <see cref="IQueryable" />.
        /// </returns>
        public Expression Expression
        {
            get { return RepositoryQuery.Expression; }
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="IQueryable" /> is executed.
        /// </summary>
        /// <returns>
        /// A <see cref="Type" /> that represents the type of the element(s) that are returned when the expression tree associated with this object is executed.
        /// </returns>
        public Type ElementType
        {
            get { return RepositoryQuery.ElementType; }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <returns>
        /// The <see cref="IQueryProvider" /> that is associated with this data source.
        /// </returns>
        public IQueryProvider Provider
        {
            get { return RepositoryQuery.Provider; }
        }

        #endregion

        #region Implementation of IRepository<TEntity>
        /// <summary>
        /// Marks the entity instance to be saved to the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be saved
        /// to the database.</param>
        /// <remarks>Implementors of this method must handle the Insert scenario.</remarks>
        public abstract void Add(TEntity entity);

        /// <summary>
        /// Marks the changes of an existing entity to be saved to the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be
        /// updated in the database.</param>
        /// <remarks>Implementors of this method must handle the Update scneario. </remarks>
        public abstract void Save(TEntity entity);

        /// <summary>
        /// Marks the entity instance to be deleted from the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be deleted.</param>
        public abstract void Delete(TEntity entity);

        /// <summary>
        /// Detaches a instance from the repository.
        /// </summary>
        /// <param name="entity">The entity instance, currently being tracked via the repository, to detach.</param>
        /// <exception cref="NotImplementedException">Implentors should throw the NotImplementedException if Detaching
        /// entities is not supported.</exception>
        public abstract void Detach(TEntity entity);

        /// <summary>
        /// Attaches a detached entity, previously detached via the <see cref="IRepository{TEntity}.Detach"/> method.
        /// </summary>
        /// <param name="entity">The entity instance to attach back to the repository.</param>
        /// <exception cref="NotImplementedException">Implentors should throw the NotImplementedException if Attaching
        /// entities is not supported.</exception>
        public abstract void Attach(TEntity entity);

        /// <summary>
        /// Refreshes a entity instance.
        /// </summary>
        /// <param name="entity">The entity to refresh.</param>
        /// <exception cref="NotImplementedException">Implementors should throw the NotImplementedException if Refreshing
        /// entities is not supported.</exception>
        public abstract void Refresh(TEntity entity);

        /// <summary>
        /// Instructs the repository to eager load a child entities. 
        /// </summary>
        /// <param name="path">The path of the child entities to eager load.</param>
        /// <remarks>Implementors should throw a <see cref="NotSupportedException"/> if the underling provider
        /// does not support eager loading of entities</remarks>
        public abstract IRepository<TEntity> With(Expression<Func<TEntity, object>> path);

        /// <summary>
        /// Instructs the repository to eager load entities that may be in the repository's association path.
        /// </summary>
        /// <param name="path">The path of the child entities to eager load.</param>
        /// <remarks>Implementors should throw a <see cref="NotSupportedException"/> if the underling provider
        /// does not support eager loading of entities</remarks>
		public abstract IRepository<TEntity> With<T>(Expression<Func<T, object>> path);

		/// <summary>
		/// Instructs the repository to cache the following query.
		/// </summary>
		/// <param name="cachedQueryName">string. The name to give to the cached query.</param>
		/// <remarks>Implementors should return the repository if caching is not supported.</remarks>
    	public abstract IRepository<TEntity> Cached(string cachedQueryName);

    	/// <summary>
        /// Defines the service context under which the repository will execute.
        /// </summary>
        /// <typeparam name="TService">The service type that defines the context of the repository.</typeparam>
        /// <returns>The same <see cref="IRepository{TEntity}"/> instance.</returns>
        /// <remarks>
        /// Gets all fetching strategies define for a service for the current type and configures the
        /// repository to use that fetching strategy. 
        /// </remarks>
        public IRepository<TEntity> For<TService>()
        {
            var strategies = ServiceLocator.Current
                    .GetAllInstances<IFetchingStrategy<TEntity, TService>>();
            if (strategies != null && strategies.Count() > 0)
                strategies.ForEach(x => x.Define(this));
            return this;
        }

    	/// <summary>
    	/// Sets the batch size on the repository
    	/// </summary>
    	/// <param name="size"></param>
    	public abstract IRepository<TEntity> SetBatchSize(int size);

    	/// <summary>
        /// Querries the repository based on the provided specification and returns results that
        /// are only satisfied by the specification.
        /// </summary>
        /// <param name="specification">A <see cref="ISpecification{TEntity}"/> instnace used to filter results
        /// that only satisfy the specification.</param>
        /// <returns>A <see cref="IEnumerable{TEntity}"/> that can be used to enumerate over the results
        /// of the query.</returns>
        public IEnumerable<TEntity> Query(ISpecification<TEntity> specification)
        {
            return RepositoryQuery.Where(specification.Predicate).AsQueryable();
        }
        #endregion
    }
}