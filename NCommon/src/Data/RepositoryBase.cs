#region license
//Copyright 2010 Ritesh Rao 

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
using NCommon.Data.Language;
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
        /// <summary>
        /// Gets the <see cref="IQueryable{TEntity}"/> used by the <see cref="RepositoryBase{TEntity}"/> 
        /// to execute Linq queries.
        /// </summary>
        /// <value>A <see cref="IQueryable{TEntity}"/> instance.</value>
        /// <remarks>
        /// Inheritors of this base class should return a valid non-null <see cref="IQueryable{TEntity}"/> instance.
        /// </remarks>
        protected abstract IQueryable<TEntity> RepositoryQuery { get; }

        /// <summary>
        /// Gets the <see cref="IUnitOfWork"/> that the repository should use.
        /// </summary>
        /// <typeparam name="TUnitOfWork">A compatible unit of work instance.</typeparam>
        /// <returns></returns>
        protected virtual TUnitOfWork GetCurrentUnitOfWork<TUnitOfWork> () where TUnitOfWork : IUnitOfWork
        {
            var currentScope = UnitOfWorkManager.CurrentUnitOfWork;
            Guard.Against<InvalidOperationException>(currentScope == null,
                                                     "No compatible UnitOfWork was found. Please start a compatible UnitOfWorkScope before " +
                                                     "using the repository.");

            Guard.TypeOf<TUnitOfWork>(currentScope,
                                              "The current UnitOfWork instance is not compatible with the repository. " +
                                              "Please start a compatible unit of work before using the repository.");
            return ((TUnitOfWork)currentScope);
        }

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
        public abstract void Detach(TEntity entity);

        /// <summary>
        /// Attaches a detached entity, previously detached via the <see cref="IRepository{TEntity}.Detach"/> method.
        /// </summary>
        /// <param name="entity">The entity instance to attach back to the repository.</param>
        public abstract void Attach(TEntity entity);

        /// <summary>
        /// Refreshes a entity instance.
        /// </summary>
        /// <param name="entity">The entity to refresh.</param>
        public abstract void Refresh(TEntity entity);

        /// <summary>
        /// Instructs the repository to eager load a child entities. 
        /// </summary>
        /// <param name="path">The path of the child entities to eager load.</param>
        public IRepository<TEntity> With(Expression<Func<TEntity, object>> path)
        {
            ApplyFetchingStrategy(new[]{path});
            return this;
        }

        /// <summary>
        /// Instructs the repository to eager load entities that may be in the repository's association path.
        /// </summary>
        /// <param name="path">The path of the child entities to eager load.</param>
		public IRepository<TEntity> With<T>(Expression<Func<T, object>> path)
        {
            ApplyFetchingStrategy(new[] { path });
            return this;    
        }

        /// <summary>
        /// Eagerly fetch associations on the entity.
        /// </summary>
        /// <param name="strategyActions">An <see cref="Action{RepositoryEagerFetchingStrategy}"/> delegate
        /// that specifies the eager fetching paths.</param>
        /// <returns>The <see cref="IRepository{TEntity}"/> instance.</returns>
        public IRepository<TEntity> Eagerly(Action<RepositoryEagerFetchingStrategy<TEntity>> strategyActions)
        {
            var strategy = new RepositoryEagerFetchingStrategy<TEntity>();
            strategyActions(strategy);
            ApplyFetchingStrategy(strategy.Paths.ToArray());
            return this;
        }

        /// <summary>
        /// When overriden by inheriting classes, applies the fetching strategies on the repository.
        /// </summary>
        /// <param name="paths">An array of <see cref="Expression"/> containing the paths to
        /// eagerly fetch.</param>
        protected abstract void ApplyFetchingStrategy(Expression[] paths);

        /// <summary>
		/// Instructs the repository to cache the following query.
		/// </summary>
		/// <param name="cachedQueryName">string. The name to give to the cached query.</param>
    	public abstract IRepository<TEntity> Cached(string cachedQueryName);

        /// <summary>
    	/// Sets the batch size on the repository
    	/// </summary>
    	/// <param name="size">int. The batch size.</param>
    	public abstract IRepository<TEntity> SetBatchSize(int size);

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
    }
}