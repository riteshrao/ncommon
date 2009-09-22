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
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace NCommon.Data.LinqToSql
{
    /// <summary>
    /// Inherits from the <see cref="RepositoryBase{TEntity}"/> class to provide an implementation of a
    /// Repository that uses Linq To SQL.
    /// </summary>
    public class LinqToSqlRepository<TEntity> : RepositoryBase<TEntity> where TEntity : class
    {
        #region fields
        private DataContext _privateDataContext;
        private readonly DataLoadOptions _loadOptions = new DataLoadOptions();
        
        #endregion

        #region .ctor
        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="LinqToSqlRepository{TEntity}"/> class.
        /// </summary>
        public LinqToSqlRepository() {}

        /// <summary>
        /// Overloaded Constructor.
        /// Creates a new instance of the <see cref="LinqToSqlRepository{TEntity}"/> instance that uses
        /// the specified DataContext.
        /// </summary>
        /// <param name="context">The <see cref="DataContext"/> instance that the repository should use.</param>
        public LinqToSqlRepository(DataContext context)
        {
            if (context == null)
                return; //ArgumentNullException is not thrown when context is null to allow a possible IoC injection.

            this._privateDataContext = context;
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the <see cref="DataContext"/> instnace that is used by the repository.
        /// </summary>
        private DataContext DataContext
        {
            get
            {
                return _privateDataContext ?? GetCurrentUnitOfWork<LinqToSqlUnitOfWork>().Context;
            }   
        }

        /// <summary>
        /// Gets the <see cref="Table"/> that represents the entity's table in the <see cref="DataContext"/>.
        /// </summary>
        private Table<TEntity> Table
        {
            get
            {
                return DataContext.GetTable<TEntity>();
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
                DataContext.LoadOptions = _loadOptions;
                return Table;
            }
        }

        /// <summary>
        /// Marks the entity instance to be saved to the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be saved
        /// to the database.</param>
        public override void Add(TEntity entity)
        {
            Table.InsertOnSubmit(entity);
        }

        /// <summary>
        /// Marks the changes of an existing entity to be saved to the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be
        /// updated in the database.</param>
        public override void Save(TEntity entity)
        {
            //Don't do anything as Linq to SQL uses Change Tracking to figure out updated entities.
        }

        /// <summary>
        /// Marks the entity instance to be deleted from the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be deleted.</param>
        public override void Delete(TEntity entity)
        {
            Table.DeleteOnSubmit(entity);
        }

        /// <summary>
        /// Detaches a instance from the repository.
        /// </summary>
        /// <param name="entity">The entity instance, currently being tracked via the repository, to detach.</param>
        /// <exception cref="NotImplementedException">Implentors should throw the NotImplementedException if Detaching
        /// entities is not supported.</exception>
        public override void Detach(TEntity entity)
        {
            throw new NotSupportedException(); //Entities auto-detach when the Context is disposed off.
        }

        /// <summary>
        /// Attaches a detached entity, previously detached via the <see cref="IRepository{TEntity}.Detach"/> method.
        /// </summary>
        /// <param name="entity">The entity instance to attach back to the repository.</param>
        /// <exception cref="NotImplementedException">Implentors should throw the NotImplementedException if Attaching
        /// entities is not supported.</exception>
        public override void Attach(TEntity entity)
        {
            Table.Attach(entity);
        }

        /// <summary>
        /// Refreshes a entity instance.
        /// </summary>
        /// <param name="entity">The entity to refresh.</param>
        /// <exception cref="NotImplementedException">Implementors should throw the NotImplementedException if Refreshing
        /// entities is not supported.</exception>
        public override void Refresh(TEntity entity)
        {
            DataContext.Refresh(RefreshMode.OverwriteCurrentValues, entity);
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
            Guard.Against<ArgumentNullException>(path == null, "Expected a non null valid path expression.");
            _loadOptions.LoadWith(path);
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

    	public override IRepository<TEntity> SetBatchSize(int size)
    	{
    		return this; //Does nothing.
    	}

    	#endregion
    }
}
