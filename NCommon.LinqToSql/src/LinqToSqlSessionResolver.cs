using System;
using System.Collections.Generic;
using System.Data.Linq;
using NCommon.Extensions;

namespace NCommon.Data.LinqToSql
{
    public class LinqToSqlSessionResolver : ILinqToSqlSessionResolver
    {
        readonly IDictionary<Type, Guid> _dataContextTypeCache = new Dictionary<Type, Guid>();
        readonly IDictionary<Guid, Func<DataContext>> _dataContextProviders = new Dictionary<Guid, Func<DataContext>>();

        public Guid GetSessionKeyFor<T>()
        {
            Guid key;
            if (!_dataContextTypeCache.TryGetValue(typeof(T), out key))
                throw new ArgumentException("No DataContext has been registered for the specified type.");
            return key;
        }

        public ILinqSession OpenSessionFor<T>()
        {
            var key = GetSessionKeyFor<T>();
            return new LinqSession(_dataContextProviders[key]());
        }

        public DataContext GetDataContextFor<T>()
        {
            Guid key;
            if (!_dataContextTypeCache.TryGetValue(typeof(T), out key))
                throw new ArgumentException("No DataContext has been registered for the specified type.");
            return _dataContextProviders[key]();
        }

        public void RegisterDataContextProvider(Func<DataContext> contextProvider)
        {
            var key = Guid.NewGuid();
            _dataContextProviders.Add(key, contextProvider);
            //Getting the data context and populating the _dataContextTypeCache
            var context = contextProvider();
            context.Mapping.GetTables().ForEach(table => _dataContextTypeCache.Add(table.RowType.Type, key));
        }

        public int DataContextsRegistered
        {
            get { return _dataContextProviders.Count; }
        }
    }
}