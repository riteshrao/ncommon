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
using System.Data.Metadata.Edm;
using System.Data.Objects;
using NCommon.Extensions;

namespace NCommon.Data.EntityFramework
{
    /// <summary>
    /// Implementation of <see cref="IEFSessionResolver"/> that resolves <see cref="IEFSession"/> instances.
    /// </summary>
    public class EFSessionResolver : IEFSessionResolver
    {
        readonly IDictionary<string, Guid> _objectContextTypeCache = new Dictionary<string, Guid>();
        readonly IDictionary<Guid, Func<ObjectContext>> _objectContexts = new Dictionary<Guid, Func<ObjectContext>>();

        /// <summary>
        /// Gets the number of <see cref="ObjectContext"/> instances registered with the session resolver.
        /// </summary>
        public int ObjectContextsRegistered
        {
            get { return _objectContexts.Count; }
        }

        /// <summary>
        /// Gets the unique ObjectContext key for a type. 
        /// </summary>
        /// <typeparam name="T">The type for which the ObjectContext key should be retrieved.</typeparam>
        /// <returns>A <see cref="Guid"/> representing the unique object context key.</returns>
        public Guid GetSessionKeyFor<T>()
        {
            var typeName = typeof (T).Name;
            Guid key;
            if (!_objectContextTypeCache.TryGetValue(typeName, out key))
                throw new ArgumentException("No ObjectContext has been registered for the specified type.");
            return key;
        }

        /// <summary>
        /// Opens a <see cref="IEFSession"/> instance for a given type.
        /// </summary>
        /// <typeparam name="T">The type for which an <see cref="IEFSession"/> is returned.</typeparam>
        /// <returns>An instance of <see cref="IEFSession"/>.</returns>
        public IEFSession OpenSessionFor<T>()
        {
            var context = GetObjectContextFor<T>();
            return new EFSession(context);
        }

        /// <summary>
        /// Gets the <see cref="ObjectContext"/> that can be used to query and update a given type.
        /// </summary>
        /// <typeparam name="T">The type for which an <see cref="ObjectContext"/> is returned.</typeparam>
        /// <returns>An <see cref="ObjectContext"/> that can be used to query and update the given type.</returns>
        public ObjectContext GetObjectContextFor<T>()
        {
            var typeName = typeof(T).Name;
            Guid key;
            if (!_objectContextTypeCache.TryGetValue(typeName, out key))
                throw new ArgumentException("No ObjectContext has been registered for the specified type.");
            return _objectContexts[key]();
        }

        /// <summary>
        /// Registers an <see cref="ObjectContext"/> provider with the resolver.
        /// </summary>
        /// <param name="contextProvider">A <see cref="Func{T}"/> of type <see cref="ObjectContext"/>.</param>
        public void RegisterObjectContextProvider(Func<ObjectContext> contextProvider)
        {
            var key = Guid.NewGuid();
            _objectContexts.Add(key, contextProvider);
            //Getting the object context and populating the _objectContextTypeCache.
            var context = contextProvider();
            var entities = context.MetadataWorkspace.GetItems<EntityType>(DataSpace.CSpace);
            entities.ForEach(entity => _objectContextTypeCache.Add(entity.Name, key));
        }
    }
}