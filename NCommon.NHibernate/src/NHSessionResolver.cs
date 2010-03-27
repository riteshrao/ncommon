using System;
using System.Collections.Generic;
using NCommon.Extensions;
using NHibernate;

namespace NCommon.Data.NHibernate
{
    /// <summary>
    /// Implementation of <see cref="INHSessionResolver"/>.
    /// </summary>
    public class NHSessionResolver : INHSessionResolver
    {
        readonly IDictionary<Type, Guid> _sessionFactoryTypeCache = new Dictionary<Type, Guid>();
        readonly IDictionary<Guid, Func<ISessionFactory>> _sessionFactories = new Dictionary<Guid, Func<ISessionFactory>>();

        /// <summary>
        /// Gets the unique <see cref="ISession"/> key for a type. 
        /// </summary>
        /// <typeparam name="T">The type for which the ObjectContext key should be retrieved.</typeparam>
        /// <returns>A <see cref="Guid"/> representing the unique object context key.</returns>
        public Guid GetSessionKeyFor<T>()
        {
            Guid factorykey;
            if (!_sessionFactoryTypeCache.TryGetValue(typeof(T), out factorykey))
                throw new ArgumentException("No ISessionFactory has been registered for the specified type.");
            return factorykey;
        }

        /// <summary>
        /// Opens a <see cref="ISession"/> instance for a given type.
        /// </summary>
        /// <typeparam name="T">The type for which an <see cref="ISession"/> is returned.</typeparam>
        /// <returns>An instance of <see cref="ISession"/>.</returns>
        public ISession OpenSessionFor<T>()
        {
            var key = GetSessionKeyFor<T>();
            return _sessionFactories[key]().OpenSession();
        }

        /// <summary>
        /// Gets the <see cref="ISessionFactory"/> that can be used to create instances of <see cref="ISession"/>
        /// to query and update the specified type..
        /// </summary>
        /// <typeparam name="T">The type for which an <see cref="ISessionFactory"/> is returned.</typeparam>
        /// <returns>An <see cref="ISessionFactory"/> that can be used to create instances of <see cref="ISession"/>
        /// to query and update the specified type.</returns>
        public ISessionFactory GetFactoryFor<T>()
        {
            Guid factorykey;
            if (!_sessionFactoryTypeCache.TryGetValue(typeof(T), out factorykey))
                throw new ArgumentException("No ISessionFactory has been registered for the specified type.");
            return _sessionFactories[factorykey]();
        }

        /// <summary>
        /// Registers an <see cref="ISessionFactory"/> provider with the resolver.
        /// </summary>
        /// <param name="factoryProvider">A <see cref="Func{T}"/> of type <see cref="ISessionFactory"/>.</param>
        public void RegisterSessionFactoryProvider(Func<ISessionFactory> factoryProvider)
        {
            Guard.Against<ArgumentNullException>(factoryProvider == null,
                                                 "Expected a non-null Func<ISessionFactory> instance.");
            var key = Guid.NewGuid();
            _sessionFactories.Add(key, factoryProvider);
            //Getting the factory and initializing populating _sessionFactoryTypeCache.
            var factory = factoryProvider();
            var classMappings = factory.GetAllClassMetadata();
            if (classMappings != null && classMappings.Count > 0)
                classMappings.ForEach(map => _sessionFactoryTypeCache
                                                 .Add(map.Value.GetMappedClass(EntityMode.Poco), key));
        }

        /// <summary>
        /// Gets the count of <see cref="ISessionFactory"/> providers registered with the resolver.
        /// </summary>
        public int SessionFactoriesRegistered
        {
            get { return _sessionFactories.Count; }
        }
    }
}