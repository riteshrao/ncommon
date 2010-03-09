using System;
using System.Data.Linq;

namespace NCommon.Data.LinqToSql
{
    public interface ILinqToSqlSessionResolver
    {
        Guid GetSessionKeyFor<T>();
        ILinqSession OpenSessionFor<T>();
        DataContext GetDataContextFor<T>();
        void RegisterDataContextProvider(Func<DataContext> contextProvider);
        int DataContextsRegistered { get; }
    }
}