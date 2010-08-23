using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Reflection;

namespace NCommon.Data.EntityFramework
{
    internal class EFSession : IEFSession
    {
        /// <summary>
        ///   Internal implementation of the <see cref = "IEFSession" /> interface.
        /// </summary>
        bool _disposed;
        readonly ObjectContext _context;

        /// <summary>
        ///   Default Constructor.
        ///   Creates a new instance of the <see cref = "EFSession" /> class.
        /// </summary>
        /// <param name = "context"></param>
        public EFSession(ObjectContext context)
        {
            Guard.Against<ArgumentNullException>(context == null, "Expected a non-null ObjectContext instance.");
            _context = context;
        }

        /// <summary>
        ///   Gets the underlying <see cref = "ObjectContext" />
        /// </summary>
        public ObjectContext Context
        {
            get { return _context; }
        }

        /// <summary>
        ///   Gets the Connection used by the <see cref = "ObjectContext" />
        /// </summary>
        public IDbConnection Connection
        {
            get { return _context.Connection; }
        }

#if EF_1_0
        readonly IDictionary<Type, PropertyInfo> _entitySetProperties = new Dictionary<Type, PropertyInfo>();

        string GetEntitySetName<T>() where T : class
        {
            if (!_entitySetProperties.ContainsKey(typeof(T)))
                DiscoverEntitySet<T>();
            return _entitySetProperties[typeof (T)].Name;
        }

        void DiscoverEntitySet<T>()
        {
            var matchingType = typeof(ObjectQuery<T>);
            foreach (var property in _context.GetType().GetProperties())
            {
                if (property.PropertyType.IsGenericType &&
                    matchingType.IsAssignableFrom(property.PropertyType))
                {
                    _entitySetProperties.Add(typeof(T), property);
                    return;
                }
            }
            throw new InvalidOperationException("An ObjectQuery for entity type " + typeof (T).FullName +
                                                " could not be found on the ObjectContext registered with the UnitOfWork.");
        }

        /// <summary>
        /// Creates an <see cref="ObjectQuery"/> of <typeparamref name="T"/> that can be used
        /// to query the entity.
        /// </summary>
        /// <typeparam name="T">The entityt type to query.</typeparam>
        /// <returns>A <see cref="ObjectQuery{T}"/> instance.</returns>
        public ObjectQuery<T> CreateQuery<T>() where T : class
        {
            if (!_entitySetProperties.ContainsKey(typeof(T)))
                DiscoverEntitySet<T>();
            return (ObjectQuery<T>) _entitySetProperties[typeof (T)].GetValue(_context, null);
        }

        /// <summary>
        ///   Adds a transient instance to the context associated with the session.
        /// </summary>
        /// <param name = "entity"></param>
        public void Add<T>(T entity) where T : class
        {
            _context.AddObject(GetEntitySetName<T>(), entity);
        }

        /// <summary>
        /// Deletes an entity instance from the context.
        /// </summary>
        /// <param name="entity"></param>
        public void Delete<T>(T entity) where T : class
        {
            _context.DeleteObject(entity);
        }

        /// <summary>
        /// Attaches an entity to the context. Changes to the entityt will be tracked by the underlying <see cref="ObjectContext"/>
        /// </summary>
        /// <param name="entity"></param>
        public void Attach<T>(T entity) where T : class
        {
            _context.Attach((IEntityWithKey) entity);
        }

        /// <summary>
        /// Detaches an entity from the context. Changes to the entity will not be tracked by the underlying <see cref="ObjectContext"/>.
        /// </summary>
        /// <param name="entity"></param>
        public void Detach<T>(T entity) where T : class
        {
            _context.Detach(entity);
        }

        /// <summary>
        /// Refreshes an entity.
        /// </summary>
        /// <param name="entity"></param>
        public void Refresh<T>(T entity) where T : class
        {
            _context.Refresh(RefreshMode.StoreWins, entity);
        }

        /// <summary>
        /// Saves changes made to the object context to the database.
        /// </summary>
        public void SaveChanges()
        {
            _context.SaveChanges(true);
        }
#else
        
        readonly Dictionary<Type, object> _objectSets = new Dictionary<Type, object>();

        ObjectSet<T> GetObjectSet<T>() where T : class
        {
            object set = null;
            if (!_objectSets.TryGetValue(typeof (T), out set))
            {
                set = _context.CreateObjectSet<T>();
                _objectSets.Add(typeof (T), set);
            }
            return (ObjectSet<T>) set;
        }

        /// <summary>
        ///   Adds a transient instance to the context associated with the session.
        /// </summary>
        /// <param name = "entity"></param>
        public void Add<T>(T entity) where T : class
        {
            GetObjectSet<T>().AddObject(entity);
        }

        /// <summary>
        /// Deletes an entity instance from the context.
        /// </summary>
        /// <param name="entity"></param>
        public void Delete<T>(T entity) where T : class
        {
            GetObjectSet<T>().DeleteObject(entity);
        }

        /// <summary>
        /// Attaches an entity to the context. Changes to the entityt will be tracked by the underlying <see cref="ObjectContext"/>
        /// </summary>
        /// <param name="entity"></param>
        public void Attach<T>(T entity) where T : class
        {
            //If the entity implementes the IEntityWithKey interface then we should use Context's Attach metho
            //instead of the set's Attach. Getting an exception 
            //"Mapping and metadata information could not be found for EntityType 'System.Data.Objects.DataClasses.IEntityWithKey"
            //when using set's Attach.
            var entityWithKey = typeof (IEntityWithKey);
            if (entityWithKey.IsAssignableFrom(entity.GetType()))
            {
                _context.Attach((IEntityWithKey)entity);
                return;
            }

            GetObjectSet<T>().Attach(entity);
            _context.DetectChanges(); //Required for POCO entities
        }

        /// <summary>
        /// Detaches an entity from the context. Changes to the entity will not be tracked by the underlying <see cref="ObjectContext"/>.
        /// </summary>
        /// <param name="entity"></param>
        public void Detach<T>(T entity) where T : class
        {
            GetObjectSet<T>().Detach(entity);
        }

        /// <summary>
        /// Refreshes an entity.
        /// </summary>
        /// <param name="entity"></param>
        public void Refresh<T>(T entity) where T : class
        {
            _context.Refresh(RefreshMode.StoreWins, entity);
        }

        /// <summary>
        /// Creates an <see cref="ObjectQuery"/> of <typeparamref name="T"/> that can be used
        /// to query the entity.
        /// </summary>
        /// <typeparam name="T">The entityt type to query.</typeparam>
        /// <returns>A <see cref="ObjectQuery{T}"/> instance.</returns>
        public ObjectQuery<T> CreateQuery<T>() where T : class
        {
            return _context.CreateObjectSet<T>();
        }

        /// <summary>
        ///   Saves changes made to the object context to the database.
        /// </summary>
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
#endif
        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Disposes off the managed and un-managed resources used.
        /// </summary>
        /// <param name = "disposing"></param>
        void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            if (_disposed)
                return;

            _context.Dispose();
            _disposed = true;
        }
    }
}