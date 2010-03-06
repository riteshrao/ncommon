using System;
using System.Collections.Generic;
using NCommon.Extensions;
using NHibernate;

namespace NCommon.Data.NHibernate
{
    public class NHSessionResolver : INHSessionResolver
    {
        readonly IDictionary<Type, Guid> _sessionFactoryTypeCache = new Dictionary<Type, Guid>();
        readonly IDictionary<Guid, Func<ISessionFactory>> _sessionFactories = new Dictionary<Guid, Func<ISessionFactory>>();

        public Guid GetSessionKeyFor<T>()
        {
            Guid factorykey;
            if (!_sessionFactoryTypeCache.TryGetValue(typeof(T), out factorykey))
                throw new ArgumentException("No ISessionFactory has been registered for the specified type.");
            return factorykey;
        }

        public ISession OpenSessionFor<T>()
        {
            var key = GetSessionKeyFor<T>();
            return _sessionFactories[key]().OpenSession();
        }

        public void RegisterSessionFactoryProvider(Func<ISessionFactory> factoryProvider)
        {
            var key = Guid.NewGuid();
            _sessionFactories.Add(key, factoryProvider);
            //Getting the factory and initializing populating _sessionFactoryTypeCache.
            var factory = factoryProvider();
            var classMappings = factory.GetAllClassMetadata();
            classMappings.ForEach(map => _sessionFactoryTypeCache
                    .Add(map.Value.GetMappedClass(EntityMode.Poco), key));
        }
    }
}