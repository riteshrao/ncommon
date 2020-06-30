using NCommon.DataServices.Transactions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NCommon.ObjectAccess
{
    /// <summary>
    /// Fetching strategy wrapper for a IRepository implemenation.
    /// </summary>
    /// <typeparam name="TRepository">The type of repository to wrap.</typeparam>
    /// <typeparam name="TEntity">The entity type of the repository.</typeparam>
    public abstract class RepositoryWrapperBase<TRepository, TEntity> : IRepository<TEntity> where TRepository : IRepository<TEntity>
    {
        readonly TRepository _rootRootRepository;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="RepositoryWrapperBase{TRepository,TEntity}"/> class.
        /// </summary>
        /// <param name="rootRootRepository">The <see cref="IRepository{TEntity}"/> instance to wrap.</param>
        protected RepositoryWrapperBase(TRepository rootRootRepository)
        {
            _rootRootRepository = rootRootRepository;
        }

        ///<summary>
        /// Gets the <see cref="IRepository{TEntity}"/> instnace that this RepositoryWrapperBase wraps.
        ///</summary>
        /// <value>The wrapped <see cref="IRepository{TEntity}"/> instance</value>
        public virtual TRepository RootRepository
        {
            get { return _rootRootRepository; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public virtual IEnumerator<TEntity> GetEnumerator()
        {
            return _rootRootRepository.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _rootRootRepository.GetEnumerator();
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.Expressions.Expression"/> that is associated with this instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </returns>
        public virtual Expression Expression
        {
            get { return _rootRootRepository.Expression; }
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable"/> is executed.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of the element(s) that are returned when the expression tree associated with this object is executed.
        /// </returns>
        public virtual Type ElementType
        {
            get { return _rootRootRepository.ElementType; }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.IQueryProvider"/> that is associated with this data source.
        /// </returns>
        public virtual IQueryProvider Provider
        {
            get { return _rootRootRepository.Provider; }
        }

        /// <summary>
        /// Gets the a <see cref="IUnitOfWork"/> of <typeparamref name="T"/> that
        /// the repository will use to query the underlying store.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IUnitOfWork"/> implementation to retrieve.</typeparam>
        /// <returns>The <see cref="IUnitOfWork"/> implementation.</returns>
        public virtual T UnitOfWork<T>() where T : IUnitOfWork
        {
            return _rootRootRepository.UnitOfWork<T>();
        }

        /// <summary>
        /// Adds a transient instance of <paramref name="entity"/> to be tracked
        /// and persisted by the repository.
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Add(TEntity entity)
        {
            _rootRootRepository.Add(entity);
        }

        /// <summary>
        /// Marks the changes of an existing entity to be saved to the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be
        /// updated in the database.</param>
        /// <remarks>Implementors of this method must handle the Update scneario. </remarks>
        public virtual void Delete(TEntity entity)
        {
            _rootRootRepository.Delete(entity);
        }

        /// <summary>
        /// Detaches a instance from the repository.
        /// </summary>
        /// <param name="entity">The entity instance, currently being tracked via the repository, to detach.</param>
        /// <exception cref="NotSupportedException">Implentors should throw the NotImplementedException if Detaching
        /// entities is not supported.</exception>
        public virtual void Detach(TEntity entity)
        {
            _rootRootRepository.Detach(entity);
        }

        /// <summary>
        /// Attaches a detached entity, previously detached via the <see cref="IRepository{TEntity}.Detach"/> method.
        /// </summary>
        /// <param name="entity">The entity instance to attach back to the repository.</param>
        /// <exception cref="NotSupportedException">Implentors should throw the NotImplementedException if Attaching
        /// entities is not supported.</exception>
        public virtual void Attach(TEntity entity)
        {
            _rootRootRepository.Attach(entity);
        }

        /// <summary>
        /// Refreshes a entity instance.
        /// </summary>
        /// <param name="entity">The entity to refresh.</param>
        /// <exception cref="NotSupportedException">Implementors should throw the NotImplementedException if Refreshing
        /// entities is not supported.</exception>
        public virtual void Refresh(TEntity entity)
        {
            _rootRootRepository.Refresh(entity);
        }

        /// <summary>
        /// Querries the repository based on the provided specification and returns results that
        /// are only satisfied by the specification.
        /// </summary>
        /// <param name="specification">A <see cref="ISpecification{TEntity}"/> instnace used to filter results
        /// that only satisfy the specification.</param>
        /// <returns>A <see cref="IEnumerable{TEntity}"/> that can be used to enumerate over the results
        /// of the query.</returns>
        public virtual IEnumerable<TEntity> Query(ISpecification<TEntity> specification)
        {
            return _rootRootRepository.Query(specification);
        }

        /// <summary>
        /// Defines the service context under which the repository will execute.
        /// </summary>
        /// <typeparam name="TService">The service type that defines the context of the repository.</typeparam>
        /// <returns>The same <see cref="IRepository{TEntity}"/> instance.</returns>
        /// <remarks>
        /// Implementors should perform context specific actions within this method call and return
        /// the exact same instance.
        /// </remarks>
        public IQueryable<TEntity> For<TService>()
        {
            return _rootRootRepository.For<TService>();
        }
    }
}