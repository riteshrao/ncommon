using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using NCommon.Extensions;

namespace NCommon.Data.EntityFramework
{
    public class EFSessionResolver : IEFSessionResolver
    {
        readonly IDictionary<string, Guid> _objectContextTypeCache = new Dictionary<string, Guid>();
        readonly IDictionary<Guid, Func<ObjectContext>> _objectContexts = new Dictionary<Guid, Func<ObjectContext>>();

        public int ObjectContextsRegistered
        {
            get { return _objectContexts.Count; }
        }

        public Guid GetSessionKeyFor<T>()
        {
            var typeName = typeof (T).FullName;
            Guid key;
            if (!_objectContextTypeCache.TryGetValue(typeName, out key))
                throw new ArgumentException("No ObjectContext has been registered for the specified type.");
            return key;
        }

        public IEFSession OpenSessionFor<T>()
        {
            var key = GetSessionKeyFor<T>();
            return new EFSession(_objectContexts[key]());
        }

        public ObjectContext GetObjectContextFor<T>()
        {
            var typeName = typeof(T).FullName;
            Guid key;
            if (!_objectContextTypeCache.TryGetValue(typeName, out key))
                throw new ArgumentException("No ObjectContext has been registered for the specified type.");
            return _objectContexts[key]();
        }

        public void RegisterObjectContextProvider(Func<ObjectContext> contextProvider)
        {
            var key = Guid.NewGuid();
            _objectContexts.Add(key, contextProvider);
            //Getting the object context and populating the _objectContextTypeCache.
            var context = contextProvider();
            var entities = context.MetadataWorkspace.GetItems<EntityType>(DataSpace.CSpace);
            entities.ForEach(entity => _objectContextTypeCache.Add(entity.FullName, key));
        }
    }
}