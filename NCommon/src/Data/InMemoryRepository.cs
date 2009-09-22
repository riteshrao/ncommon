using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NCommon.Data
{
	public class InMemoryRepository<TEntity> : RepositoryBase<TEntity>
	{
		readonly IList<TEntity> _internal;

		public InMemoryRepository(IList<TEntity> list)
		{
			_internal = list;
		}

		protected override IQueryable<TEntity> RepositoryQuery
		{
			get { return _internal.AsQueryable(); }
		}

		public override void Add(TEntity entity)
		{
			_internal.Add(entity);
		}

		public override void Save(TEntity entity)
		{
			_internal.Add(entity);
		}

		public override void Delete(TEntity entity)
		{
			_internal.Remove(entity);
		}

		public override void Detach(TEntity entity)
		{
			return;
		}

		public override void Attach(TEntity entity)
		{
			return;
		}

		public override void Refresh(TEntity entity)
		{
			return;
		}

		public override IRepository<TEntity> With(Expression<Func<TEntity, object>> path)
		{
			return this;
		}

		public override IRepository<TEntity> With<T>(Expression<Func<T, object>> path)
		{
			return this;
		}

		public override IRepository<TEntity> Cached(string cachedQueryName)
		{
			return this;
		}

		public override IRepository<TEntity> SetBatchSize(int size)
		{
			return this;
		}
	}
}