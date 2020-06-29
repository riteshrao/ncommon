using NCommon.ObjectAccess;

namespace NCommon.LinqToSql
{
    public interface ILinqToSqlFetchingRepository<TEntity, TRelated> : IRepository<TEntity> where TEntity : class
    {
        LinqToSqlRepository<TEntity> RootRepository { get; }
    }
}