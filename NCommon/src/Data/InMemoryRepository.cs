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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NCommon.Data
{
    /// <summary>
    /// An implementation of <see cref="RepositoryBase{TEntity}"/> that uses an inmemory
    /// collection.
    /// </summary>
    /// <typeparam name="TEntity">The entity type for which this repository was created.</typeparam>
    /// <remarks>This class can be used in Unit tests to represent an in memory repository.</remarks>
	public class InMemoryRepository<TEntity> : RepositoryBase<TEntity>
	{
		readonly IList<TEntity> _internal;

        /// <summary>
        /// Default Constructor.
        /// Creats a new instance of the <see cref="InMemoryRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="list">An optional list pre-populated with entities.</param>
		public InMemoryRepository(IList<TEntity> list)
		{
			_internal = list ?? new List<TEntity>() ;
		}

        /// <summary>
        /// Gets the <see cref="IQueryable{TEntity}"/> used by the <see cref="RepositoryBase{TEntity}"/> 
        /// to execute Linq queries.
        /// </summary>
        /// <value>A <see cref="IQueryable{TEntity}"/> instance.</value>
		protected override IQueryable<TEntity> RepositoryQuery
		{
			get { return _internal.AsQueryable(); }
		}

        /// <summary>
        /// Marks the changes of an existing entity to be saved to the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be
        /// updated in the database.</param>
        /// <remarks>Implementors of this method must handle the Update scneario. </remarks>
		public override void Save(TEntity entity)
		{
			_internal.Add(entity);
		}

        /// <summary>
        /// Marks the entity instance to be deleted from the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be deleted.</param>
		public override void Delete(TEntity entity)
		{
			_internal.Remove(entity);
		}

        /// <summary>
        /// Detaches a instance from the repository.
        /// </summary>
        /// <param name="entity">The entity instance, currently being tracked via the repository, to detach.</param>
		public override void Detach(TEntity entity)
		{
			return;
		}

        /// <summary>
        /// Attaches a detached entity, previously detached via the <see cref="IRepository{TEntity}.Detach"/> method.
        /// </summary>
        /// <param name="entity">The entity instance to attach back to the repository.</param>
		public override void Attach(TEntity entity)
		{
			return;
		}

        /// <summary>
        /// Refreshes a entity instance.
        /// </summary>
        /// <param name="entity">The entity to refresh.</param>
		public override void Refresh(TEntity entity)
		{
			return;
		}


        /// <summary>
        /// When overriden by inheriting classes, applies the fetching strategies on the repository.
        /// </summary>
        /// <param name="paths">An array of <see cref="Expression"/> containing the paths to
        /// eagerly fetch.</param>
        protected override void ApplyFetchingStrategy(Expression[] paths)
        {
            return;
        }

        /// <summary>
        /// Instructs the repository to cache the following query.
        /// </summary>
        /// <param name="cachedQueryName">string. The name to give to the cached query.</param>
		public override IRepository<TEntity> Cached(string cachedQueryName)
		{
			return this;
		}

        /// <summary>
        /// Sets the batch size on the repository
        /// </summary>
        /// <param name="size">int. The batch size.</param>
		public override IRepository<TEntity> SetBatchSize(int size)
		{
			return this;
		}
	}
}