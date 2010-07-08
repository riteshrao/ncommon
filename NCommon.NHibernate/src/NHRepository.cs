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
using Microsoft.Practices.ServiceLocation;
using NCommon.Expressions;
using NCommon.Extensions;
using NHibernate;
using NHibernate.Linq;
using System.Linq.Expressions;
using NHibernate.Transform;

namespace NCommon.Data.NHibernate
{
    /// <summary>
    /// Inherits from the <see cref="RepositoryBase{TEntity}"/> class to provide an implementation of a
    /// repository that uses NHibernate.
    /// </summary>
    public class NHRepository<TEntity> : RepositoryBase<TEntity>
    {
    	public class WithDistinctRoot : NHRepository<TEntity>
        {
    	    public WithDistinctRoot()
    	    {
    	        _resultTransformers = new[] {Transformers.DistinctRootEntity};
    	    }
        }

        int _batchSize = -1;
        readonly ISession _privateSession;
        bool _enableCached;
        string _cachedQueryName;
        readonly List<string> _expands = new List<string>();
        IResultTransformer[] _resultTransformers;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="NHRepository{TEntity}"/> class.
        /// </summary>
        public NHRepository ()
        {
            if (ServiceLocator.Current == null)
                return;

            var sessions = ServiceLocator.Current.GetAllInstances<ISession>();
            if (sessions != null && sessions.Count() > 0)
                _privateSession = sessions.FirstOrDefault();
        }

        /// <summary>
        /// Gets the <see cref="ISession"/> instnace that is used by the repository.
        /// </summary>
        private ISession Session
        {
            get
            {
                return _privateSession ?? GetCurrentUnitOfWork<NHUnitOfWork>().GetSession<TEntity>();
            }
        }

        /// <summary>
        /// Gets the <see cref="IQueryable{TEntity}"/> used by the <see cref="RepositoryBase{TEntity}"/> 
        /// to execute Linq queries.
        /// </summary>
        /// <value>A <see cref="IQueryable{TEntity}"/> instance.</value>
        /// <remarks>
        /// Inheritos of this base class should return a valid non-null <see cref="IQueryable{TEntity}"/> instance.
        /// </remarks>
        protected override IQueryable<TEntity> RepositoryQuery
        {
            get
            {
                if (_batchSize > -1)
            	    Session.SetBatchSize(_batchSize); //Done before every query.
                var query = Session.Linq<TEntity>();
            	var nhQuery = query as INHibernateQueryable;

                if (_resultTransformers != null && _resultTransformers.Length > 0)
                    _resultTransformers.ForEach(transformer => 
                        nhQuery.QueryOptions.RegisterCustomAction(x => x.SetResultTransformer(transformer)));

                if (_expands.Count > 0)
                    _expands.ForEach(x => nhQuery.QueryOptions.AddExpansion(x));

				if (_enableCached)
				{
					nhQuery.QueryOptions.SetCachable(true);
					nhQuery.QueryOptions.SetCacheMode(CacheMode.Normal);
					nhQuery.QueryOptions.SetCacheRegion(_cachedQueryName);
				}

				//Resetting cache variables once IQueryable has been built.
            	_enableCached = false;
            	_cachedQueryName = null;
                return query;
            }
        }

        /// <summary>
        /// Marks the changes of an existing entity to be saved to the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be
        /// updated in the database.</param>
        public override void Save(TEntity entity)
        {
			Session.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Marks the entity instance to be deleted from the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be deleted.</param>
        public override void Delete(TEntity entity)
        {
            Session.Delete(entity);
        }

        /// <summary>
        /// Detaches a instance from the repository.
        /// </summary>
        /// <param name="entity">The entity instance, currently being tracked via the repository, to detach.</param>
        public override void Detach(TEntity entity)
        {
            Session.Evict(entity);
        }

        /// <summary>
        /// Attaches a detached entity, previously detached via the <see cref="IRepository{TEntity}.Detach"/> method.
        /// </summary>
        /// <param name="entity">The entity instance to attach back to the repository.</param>
        public override void Attach(TEntity entity)
        {
            Session.Lock(entity, LockMode.None);
        }

        /// <summary>
        /// Refreshes a entity instance.
        /// </summary>
        /// <param name="entity">The entity to refresh.</param>
        public override void Refresh(TEntity entity)
        {
            Session.Refresh(entity, LockMode.None);
        }

        /// <summary>
        /// When overriden by inheriting classes, applies the fetching strategies on the repository.
        /// </summary>
        /// <param name="paths">An array of <see cref="RepositoryBase{TEntity}.Expression"/> containing the paths to
        /// eagerly fetch.</param>
        protected override void ApplyFetchingStrategy(Expression[] paths)
        {
            Guard.Against<ArgumentNullException>(paths == null || paths.Length == 0,
                                                 "Expected a non-null and non-empty array of Expression instances " +
                                                 "representing the paths to eagerly load.");

            var currentPath = string.Empty;
            paths.ForEach(path =>
            {
                var visitor = new MemberAccessPathVisitor();
                visitor.Visit(path);
                currentPath = !string.IsNullOrEmpty(currentPath) ?
                    currentPath + "." + visitor.Path : visitor.Path;
                _expands.Add(currentPath);
            });
        }

		/// <summary>
		/// Instructs the repository to cache the following query.
		/// </summary>
		/// <param name="cachedQueryName">string. The name of the cached query.</param>
		/// <returns></returns>
    	public override IRepository<TEntity> Cached(string cachedQueryName)
    	{
			_enableCached = true;
			_cachedQueryName = cachedQueryName;
			return this;
    	}

        /// <summary>
        /// Sets a batch size on the repository.
        /// </summary>
        /// <param name="size">int. A positive integer representing the batch size.</param>
        /// <remarks>Use this property when persisteing large amounts of data to batch insert statements.</remarks>
    	public override IRepository<TEntity> SetBatchSize(int size)
    	{
    		Guard.Against<ArgumentOutOfRangeException>(size < 0, "BatchSize cannot be set to a value lesser than 0.");
    		_batchSize = size;
    		return this;
    	}
    }
}
