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
using NCommon.Expressions;
using NHibernate;
using NHibernate.Linq;
using System.Linq.Expressions;

namespace NCommon.Data.NHibernate
{
    /// <summary>
    /// Inherits from the <see cref="RepositoryBase{TEntity}"/> class to provide an implementation of a
    /// Repository that uses NHibernate.
    /// </summary>
    public class NHRepository<TEntity> : RepositoryBase<TEntity>
    {
        #region fields
    	private int _batchSize = -1;
    	private readonly ISession _privateSession;
    	private readonly List<string> _expands = new List<string>();
    	private bool _enableCached;
    	private string _cachedQueryName;

    	#endregion

        #region .ctor
        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="NHRepository{TEntity}"/> class.
        /// </summary>
        public NHRepository () {}

        /// <summary>
        /// Overloaded Constructor.
        /// Creates a new instance of the <see cref="NHRepository{TEntity}"/> class
        /// that uses the specified NHiberante session.
        /// </summary>
        /// <param name="session">The <see cref="ISession"/> instance that the repository should use.</param>
        public NHRepository(ISession session)
        {
            //ArgumentNullException is not thrown when session is null to allow a possible IoC injection.
            _privateSession = session; 
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the <see cref="ISession"/> instnace that is used by the repository.
        /// </summary>
        private ISession Session
        {
            get
            {
                return _privateSession ?? GetCurrentUnitOfWork<NHUnitOfWork>().Session;
            }
        }
        #endregion

        #region Overrides of RepositoryBase<TEntity>
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
        /// Marks the entity instance to be saved to the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be saved
        /// to the database.</param>
        public override void Add(TEntity entity)
        {
            Session.Save(entity);
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
        /// <exception cref="NotImplementedException">Implentors should throw the NotImplementedException if Detaching
        /// entities is not supported.</exception>
        public override void Detach(TEntity entity)
        {
            Session.Evict(entity);
        }

        /// <summary>
        /// Attaches a detached entity, previously detached via the <see cref="IRepository{TEntity}.Detach"/> method.
        /// </summary>
        /// <param name="entity">The entity instance to attach back to the repository.</param>
        /// <exception cref="NotImplementedException">Implentors should throw the NotImplementedException if Attaching
        /// entities is not supported.</exception>
        public override void Attach(TEntity entity)
        {
            Session.Lock(entity, LockMode.None);
        }

        /// <summary>
        /// Refreshes a entity instance.
        /// </summary>
        /// <param name="entity">The entity to refresh.</param>
        /// <exception cref="NotImplementedException">Implementors should throw the NotImplementedException if Refreshing
        /// entities is not supported.</exception>
        public override void Refresh(TEntity entity)
        {
            Session.Refresh(entity, LockMode.None);
        }

        /// <summary>
        /// Instructs the repository to eager load a child entities. 
        /// </summary>
        /// <param name="path">The path of the child entities to eager load.</param>
        /// <remarks>Implementors should throw a <see cref="NotSupportedException"/> if the underling provider
        /// does not support eager loading of entities</remarks>
		public override IRepository<TEntity> With(Expression<Func<TEntity, object>> path)
        {
            return With<TEntity>(path);
        }

        /// <summary>
        /// Instructs the repository to eager load entities that may be in the repository's association path.
        /// </summary>
        /// <param name="path">The path of the child entities to eager load.</param>
        /// <remarks>Implementors should throw a <see cref="NotSupportedException"/> if the underling provider
        /// does not support eager loading of entities</remarks>
        public override IRepository<TEntity> With<T>(Expression<Func<T, object>> path)
        {
            Guard.Against<ArgumentNullException>(path == null,
                                                 "Expected a non null Expression representing a path to be eger loaded.");
            var visitor = new MemberAccessPathVisitor();
            visitor.Visit(path);
            if (typeof(T) == typeof(TEntity))
                _expands.Add(visitor.Path);
            else
            {
                //The path represents an collection association. Find the property on the target type that
                //matches a IEnumerable<T> property.
                var pathExpression = visitor.Path;
                var targetType = typeof (TEntity);
                var matchesType = typeof (IEnumerable<T>);
                var targetProperty = (from property in targetType.GetProperties()
                                      where matchesType.IsAssignableFrom(property.PropertyType)
                                      select property).FirstOrDefault();
                if (targetProperty != null)
                    pathExpression = string.Format("{0}.{1}", targetProperty.Name, pathExpression);
                _expands.Add(pathExpression);
            }
        	return this;
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

    	public override IRepository<TEntity> SetBatchSize(int size)
    	{
    		Guard.Against<ArgumentOutOfRangeException>(size < 0, "BatchSize cannot be set to a value lesser than 0.");
    		_batchSize = size;
    		return this;
    	}
    	#endregion
    }
}
