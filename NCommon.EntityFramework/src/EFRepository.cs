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
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using NCommon.Expressions;

namespace NCommon.Data.EntityFramework
{
    /// <summary>
    /// Inherits from the <see cref="RepositoryBase{TEntity}"/> class to provide an implementation of a
    /// Repository that uses Entity Framework.
    /// </summary>
    public class EFRepository<TEntity> : RepositoryBase<TEntity>
    {
        private ObjectContext _privateContext;
        private PropertyInfo _contextQueryProperty;
        private readonly List<string> _includes = new List<string>();

        /// <summary>
        /// Creates a new instance of the <see cref="EFRepository{TEntity}"/> class.
        /// </summary>
        public EFRepository()
        {
            if (ServiceLocator.Current == null) 
                return;

            var objectContexts = ServiceLocator.Current.GetAllInstances<ObjectContext>();
            if (objectContexts != null && objectContexts.Count() > 0)
                _privateContext = objectContexts.FirstOrDefault();
        }

        /// <summary>
        /// Gets the <see cref="ObjectContext"/> to be used by the repository.
        /// </summary>
        private ObjectContext Context
        {
            get
            {
                if (_privateContext != null)
                    return _privateContext;
                var unitOfWork = GetCurrentUnitOfWork<EFUnitOfWork>();
                if (_contextQueryProperty == null)
                    LoadObjectQueryPropertyAndEntitySetName(unitOfWork.GetSession<TEntity>().Context);
                return unitOfWork.GetSession<TEntity>().Context;
            }
        }

        /// <summary>
        /// Gets or sets the entity set name defined for the entity in the <see cref="Context"/> instance.
        /// </summary>
        private string EntitySetName { get; set; }

        /// <summary>
        /// Loads the <see cref="PropertyInfo"/> that can be invoked to get a <see cref="ObjectQuery{T}"/> 
        /// for the entity from the current ObjectContext instnace.
        /// </summary>
        /// <param name="context">The <see cref="ObjectContext"/> to use.</param>
        /// <returns></returns>
        private void LoadObjectQueryPropertyAndEntitySetName(ObjectContext context)
        {
            foreach (var property in context.GetType().GetProperties())
            {
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType == typeof(ObjectQuery<TEntity>))
                {
                    _contextQueryProperty = property;
                    EntitySetName = property.Name;
                    return;
                }
            }
            throw new InvalidOperationException("The repository could not find a ObjectQuery for entity type " +
                                                typeof (TEntity).FullName);
        }

        /// <summary>
        /// Gets a <see cref="ObjectQuery{TEntity}"/> instance that can be used to query the underlying
        /// data store.
        /// </summary>
        /// <returns></returns>
        private ObjectQuery<TEntity> GetQuery ()
        {
            var currentContext = this.Context;
            return (ObjectQuery<TEntity>) this._contextQueryProperty.GetValue(currentContext, null);
        }

        /// <summary>
        /// Gets the <see cref="IQueryable{TEntity}"/> used by the <see cref="RepositoryBase{TEntity}"/> 
        /// to execute Linq queries.
        /// </summary>
        /// <value>A <see cref="IQueryable{TEntity}"/> instance.</value>
        protected override IQueryable<TEntity> RepositoryQuery
        {
            get
            {
                var query = GetQuery();
                if (_includes.Count > 0)
                    _includes.ForEach(x => query = query.Include(x));
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
            Context.AddObject(this.EntitySetName, entity);
        }

        /// <summary>
        /// Marks the changes of an existing entity to be saved to the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be
        /// updated in the database.</param>
        /// <remarks>Implementors of this method must handle the Update scneario. </remarks>
        public override void Save(TEntity entity)
        {
            //Do nothing as Entity Framework uses change tracking to track changes.
        }

        /// <summary>
        /// Marks the entity instance to be deleted from the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be deleted.</param>
        public override void Delete(TEntity entity)
        {
            Context.DeleteObject(entity);
        }

        /// <summary>
        /// Detaches a instance from the repository.
        /// </summary>
        /// <param name="entity">The entity instance, currently being tracked via the repository, to detach.</param>
        /// <exception cref="NotImplementedException">Implentors should throw the NotImplementedException if Detaching
        /// entities is not supported.</exception>
        public override void Detach(TEntity entity)
        {
            Context.Detach(entity);
        }

        /// <summary>
        /// Attaches a detached entity, previously detached via the <see cref="IRepository{TEntity}.Detach"/> method.
        /// </summary>
        /// <param name="entity">The entity instance to attach back to the repository.</param>
        /// <exception cref="NotImplementedException">Implentors should throw the NotImplementedException if Attaching
        /// entities is not supported.</exception>
        public override void Attach(TEntity entity)
        {
            Context.Attach(entity as IEntityWithKey);
        }

        /// <summary>
        /// Refreshes a entity instance.
        /// </summary>
        /// <param name="entity">The entity to refresh.</param>
        /// <exception cref="NotImplementedException">Implementors should throw the NotImplementedException if Refreshing
        /// entities is not supported.</exception>
        public override void Refresh(TEntity entity)
        {
            Context.Refresh(RefreshMode.StoreWins, entity);
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
            Guard.Against<ArgumentNullException>(path == null, "Expected a non-null valid path expression.");
            var visitor = new MemberAccessPathVisitor();
            visitor.Visit(path);
            if (typeof(T) == typeof(TEntity))
                _includes.Add(visitor.Path);
            else
            {
                //The path represents an collection association. Find the property on the target type that
                //matches a IEnumerable<T> property.
                var pathExpression = visitor.Path;
                var targetType = typeof(TEntity);
                var matchesType = typeof(IEnumerable<T>);
                var targetProperty = (from property in targetType.GetProperties()
                                      where matchesType.IsAssignableFrom(property.PropertyType)
                                      select property).FirstOrDefault();
                if (targetProperty != null)
                    pathExpression = string.Format("{0}.{1}", targetProperty.Name, pathExpression);
                _includes.Add(pathExpression);
            }
        	return this;
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
        /// Sets a batch size on the repository.
        /// </summary>
        /// <param name="size">int. A positive integer representing the batch size.</param>
        /// <remarks>Use this property when persisteing large amounts of data to batch insert statements.</remarks>
    	public override IRepository<TEntity> SetBatchSize(int size)
    	{
    		return this; //Does nothing.
    	}
    }
}
