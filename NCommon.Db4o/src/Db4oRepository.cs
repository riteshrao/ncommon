using System;
using System.Linq;
using System.Linq.Expressions;
using Db4objects.Db4o;
using Db4objects.Db4o.Linq;
using NCommon.Data;

namespace NCommon.Db4o
{
    public class Db4oRepository<TEntity> : RepositoryBase<TEntity>
    {
        readonly IObjectContainer _privateContainer;

        public Db4oRepository() {}

        public Db4oRepository(IObjectContainer privateContainer)
        {
            _privateContainer = privateContainer;
        }

        public IObjectContainer ObjectContainer
        {
            get { return _privateContainer ?? GetCurrentUnitOfWork<Db4oUnitOfWork>().ObjectContainer; }
        }

        protected override IQueryable<TEntity> RepositoryQuery
        {
            get { return ObjectContainer.Cast<TEntity>().AsQueryable(); }
        }

        public override void Add(TEntity entity)
        {
            ObjectContainer.Store(entity);
        }

        public override void Save(TEntity entity)
        {
            ObjectContainer.Store(entity);
        }

        public override void Delete(TEntity entity)
        {
            ObjectContainer.Delete(entity);
        }

        public override void Detach(TEntity entity)
        {
            ObjectContainer.Ext().Purge(entity);
        }

        public override void Attach(TEntity entity)
        {
            //Noop.
        }

        public override void Refresh(TEntity entity)
        {
            ObjectContainer.Ext().Refresh(entity, 0);
        }

        public override IRepository<TEntity> With(Expression<Func<TEntity, object>> path)
        {
            //Not implemented.
            return this;
        }

        public override IRepository<TEntity> With<T>(Expression<Func<T, object>> path)
        {
            //Not implemented.
            return this;
        }

        public override IRepository<TEntity> Cached(string cachedQueryName)
        {
            //Not implemented.
            return this;
        }

        public override IRepository<TEntity> SetBatchSize(int size)
        {
            //Not implemented.
            return this;
        }
    }
}