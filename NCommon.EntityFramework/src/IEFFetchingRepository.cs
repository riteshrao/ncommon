using NCommon.ObjectAccess;

namespace NCommon.Data.EntityFramework
{
    public interface IEFFetchingRepository<TEntity, TFetch> : IRepository<TEntity> where TEntity : class
    {
        EFRepository<TEntity> RootRepository { get; }

        string FetchingPath { get; }
    }
}