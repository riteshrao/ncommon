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
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using NCommon.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace NCommon.Data.LinqToSql
{
    /// <summary>
    /// Inherits from the <see cref="RepositoryBase{TEntity}"/> class to provide an implementation of a
    /// Repository that uses Linq To SQL.
    /// </summary>
    public class LinqToSqlRepository<TEntity> : RepositoryBase<TEntity> where TEntity : class
    {
        readonly DataContext _privateDataContext;
        readonly DataLoadOptions _loadOptions = new DataLoadOptions();

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="LinqToSqlRepository{TEntity}"/> class.
        /// </summary>
        public LinqToSqlRepository()
        {
            if (ServiceLocator.Current == null)
                return;

            var contexts = ServiceLocator.Current.GetAllInstances<DataContext>();
            if (contexts != null && contexts.Count() > 0)
                _privateDataContext = contexts.FirstOrDefault();
        }

        /// <summary>
        /// Gets the <see cref="DataContext"/> instnace that is used by the repository.
        /// </summary>
        private DataContext DataContext
        {
            get
            {
                return _privateDataContext ?? UnitOfWork<LinqToSqlUnitOfWork>().GetSession<TEntity>().Context;
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
        /// Adds a transient instance of <see cref="TEntity"/> to be tracked
        /// and persisted by the repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <remarks>
        /// The Add method replaces the existing <see cref="RepositoryBase{TEntity}.Save"/> method, which will
        /// eventually be removed from the public API.
        /// </remarks>
        public override void Add(TEntity entity)
        {
            Table.InsertOnSubmit(entity);
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
            throw new NotSupportedException("LinqToSqlRepository does not support detaching entities explicitly.");
            //Entities auto-detach when the Context is disposed off.
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
    }
}
