using System;
using System.Linq;
using System.Linq.Expressions;
using Db4objects.Db4o;
using Db4objects.Db4o.Linq;
using Microsoft.Practices.ServiceLocation;
using NCommon.Data;

namespace NCommon.Data.Db4o
{
    /// <summary>
    /// Inherits from the <see cref="RepositoryBase{TEntity}"/> class to provide an implementation of a
    /// repository that uses Db4o.
    /// </summary>
    public class Db4oRepository<TEntity> : RepositoryBase<TEntity>
    {
        readonly IObjectContainer _privateContainer;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="Db4oRepository{TEntity}"/> class.
        /// </summary>
        public Db4oRepository()
        {
            _privateContainer = ServiceLocator.Current.GetAllInstances<IObjectContainer>().FirstOrDefault();
        }

        /// <summary>
        /// Gets the <see cref="IObjectContainer"/> instance that is used by the repository.
        /// </summary>
        public IObjectContainer ObjectContainer
        {
            get { return _privateContainer ?? GetCurrentUnitOfWork<Db4oUnitOfWork>().ObjectContainer; }
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
            get { return ObjectContainer.Cast<TEntity>().AsQueryable(); }
        }

        /// <summary>
        /// Marks the entity instance to be saved to the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be saved
        /// to the database.</param>
        public override void Add(TEntity entity)
        {
            ObjectContainer.Store(entity);
        }

        /// <summary>
        /// Marks the changes of an existing entity to be saved to the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be
        /// updated in the database.</param>
        public override void Save(TEntity entity)
        {
            ObjectContainer.Store(entity);
        }

        /// <summary>
        /// Marks the entity instance to be deleted from the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be deleted.</param>
        public override void Delete(TEntity entity)
        {
            ObjectContainer.Delete(entity);
        }

        /// <summary>
        /// Detaches a instance from the repository.
        /// </summary>
        /// <param name="entity">The entity instance, currently being tracked via the repository, to detach.</param>
        public override void Detach(TEntity entity)
        {
            ObjectContainer.Ext().Purge(entity);
        }

        /// <summary>
        /// Attaches a detached entity, previously detached via the <see cref="IRepository{TEntity}.Detach"/> method.
        /// </summary>
        /// <param name="entity">The entity instance to attach back to the repository.</param>
        public override void Attach(TEntity entity)
        {
            //Noop.
        }

        /// <summary>
        /// Refreshes a entity instance.
        /// </summary>
        /// <param name="entity">The entity to refresh.</param>
        public override void Refresh(TEntity entity)
        {
            ObjectContainer.Ext().Refresh(entity, 0);
        }

        /// <summary>
        /// Instructs the repository to eager load a child entities. 
        /// </summary>
        /// <param name="path">The path of the child entities to eager load.</param>
        /// <remarks>Implementors should throw a <see cref="NotSupportedException"/> if the underling provider
        /// does not support eager loading of entities</remarks>
        public override IRepository<TEntity> With(Expression<Func<TEntity, object>> path)
        {
            //Not implemented.
            return this;
        }

        /// <summary>
        /// Instructs the repository to eager load entities that may be in the repository's association path.
        /// </summary>
        /// <param name="path">The path of the child entities to eager load.</param>
        /// <remarks>Implementors should throw a <see cref="NotSupportedException"/> if the underling provider
        /// does not support eager loading of entities</remarks>
        public override IRepository<TEntity> With<T>(Expression<Func<T, object>> path)
        {
            //Not implemented.
            return this;
        }

        /// <summary>
        /// Instructs the repository to cache the following query.
        /// </summary>
        /// <param name="cachedQueryName">string. The name of the cached query.</param>
        /// <returns></returns>
        public override IRepository<TEntity> Cached(string cachedQueryName)
        {
            //Not implemented.
            return this;
        }

        /// <summary>
        /// Sets a batch size on the repository.
        /// </summary>
        /// <param name="size">int. A positive integer representing the batch size.</param>
        /// <remarks>Use this property when persisteing large amounts of data to batch insert statements.</remarks>
        public override IRepository<TEntity> SetBatchSize(int size)
        {
            //Not implemented.
            return this;
        }
    }
}