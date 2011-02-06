namespace NCommon.Data.LinqToSql
{
    public interface ILinqToSqlFetchingRepository<TEntity, TRelated> : IRepository<TEntity> where TEntity : class
    {
        LinqToSqlRepository<TEntity> RootRepository { get; }
    }
}