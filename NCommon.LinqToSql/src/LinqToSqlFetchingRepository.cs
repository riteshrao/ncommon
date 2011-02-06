namespace NCommon.Data.LinqToSql
{
    public class LinqToSqlFetchingRepository<TEntity, TRelated> : RepositoryWrapperBase<LinqToSqlRepository<TEntity>, TEntity>, ILinqToSqlFetchingRepository<TEntity, TRelated> where TEntity : class
    {
        public LinqToSqlFetchingRepository(LinqToSqlRepository<TEntity> rootRepository) : base(rootRepository) { }
    }
}