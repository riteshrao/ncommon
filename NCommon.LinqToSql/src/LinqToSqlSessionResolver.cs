#region license
//Copyright 2010 Ritesh Rao 

//Licensed under the Apache License, Version 2.0 (the "License"); 
//you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 

//http://www.apache.org/licenses/LICENSE-2.0 

//Unless required by applicable law or agreed to in writing, software 
//distributed under the License is distributed on an "AS IS" BASIS, 
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and 
//limitations under the License. 
#endregion

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

        /// <summary>
        /// Gets the unique <see cref="ILinqToSqlSession"/> key for a type. 
        /// </summary>
        /// <typeparam name="T">The type for which the ObjectContext key should be retrieved.</typeparam>
        /// <returns>A <see cref="Guid"/> representing the unique object context key.</returns>
        public Guid GetSessionKeyFor<T>()
        {
            Guid key;
            if (!_dataContextTypeCache.TryGetValue(typeof(T), out key))
                throw new ArgumentException("No DataContext has been registered for the specified type.");
            return key;
        }

        /// <summary>
        /// Gets a <see cref="ILinqToSqlSession"/> instance for a given type.
        /// </summary>
        /// <typeparam name="T">The type for which an <see cref="ILinqToSqlSession"/> is returned.</typeparam>
        /// <returns>An instance of <see cref="ILinqToSqlSession"/>.</returns>
        public ILinqToSqlSession OpenSessionFor<T>()
        {
            var key = GetSessionKeyFor<T>();
            return new LinqToSqlSession(_dataContextProviders[key]());
        }

        /// <summary>
        /// Gets the <see cref="DataContext"/> that can be used to query and update a given type.
        /// </summary>
        /// <typeparam name="T">The type for which an <see cref="DataContext"/> is returned.</typeparam>
        /// <returns>An <see cref="DataContext"/> that can be used to query and update the given type.</returns>
        public DataContext GetDataContextFor<T>()
        {
            Guid key;
            if (!_dataContextTypeCache.TryGetValue(typeof(T), out key))
                throw new ArgumentException("No DataContext has been registered for the specified type.");
            return _dataContextProviders[key]();
        }

        /// <summary>
        /// Registers an <see cref="DataContext"/> provider with the resolver.
        /// </summary>
        /// <param name="contextProvider">A <see cref="Func{T}"/> of type <see cref="DataContext"/>.</param>
        public void RegisterDataContextProvider(Func<DataContext> contextProvider)
        {
            Guard.Against<ArgumentNullException>(contextProvider == null,
                                                 "Expected a non-null Func<DataContext> instance.");
            var key = Guid.NewGuid();
            _dataContextProviders.Add(key, contextProvider);
            //Getting the data context and populating the _dataContextTypeCache
            var context = contextProvider();
            context.Mapping.GetTables().ForEach(table => _dataContextTypeCache.Add(table.RowType.Type, key));
        }

        /// <summary>
        /// Gets the count of <see cref="DataContext"/> providers registered with the resolver.
        /// </summary>
        public int DataContextsRegistered
        {
            get { return _dataContextProviders.Count; }
        }
    }
}