#region license
//Copyright 2008 Ritesh Rao 

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


using System.Collections;
using System.Web;

namespace NCommon.Storage
{
    /// <summary>
    /// Provifes an in-memory store for storing application / thread specific data.
    /// </summary>
    public abstract class Store
    {
        #region fields
        /// <summary>
        /// Lock object that can be used to sync access to the Application store.
        /// </summary>
        protected static readonly object AppStorageLock = new object();
        /// <summary>
        /// Lock object that can be used to sync acces to the Local store.
        /// </summary>
        protected static readonly object LocalStorageLock = new object();
        /// <summary>
        /// Lock object that can be used to sync acces to the Session store.
        /// </summary>
        protected static readonly object SessionStorageLock = new object();

        private static AppStorage _appStorage;
        private static LocalStorage _localStorage;
        private static SessionStorage _sessionStorage;
        #endregion

        #region properties

        /// <summary>
        /// Gets whether the current application is a web application.
        /// </summary>
        /// <value>bool. True if the current application is a web application.</value>
        public static bool IsWebApplication
        {
            get { return HttpContext.Current != null; }
        }

        /// <summary>
        /// Gets a <see cref="Store"/> implementation that can be used to store application
        /// sepcific data for the current thread / request.
        /// </summary>
        public static Store Local
        {
            get
            {
                if (_localStorage == null)
                {
                    lock (LocalStorageLock)
                    {
                        if (_localStorage == null)
                            _localStorage = new LocalStorage();
                    }
                }
                return _localStorage;
            }
        }

        /// <summary>
        /// Gets a <see cref="Store"/> instance that can be used to store application specific
        /// data for the applicatuion.
        /// </summary>
        public static Store Application
        {
            get
            {
                if (_appStorage == null)
                {
                    lock (AppStorageLock)
                    {
                        if (_appStorage == null)
                            _appStorage = new AppStorage();
                    }
                }
                return _appStorage;
            }
        }

        /// <summary>
        /// Gets a <see cref="Store"/> instance that can be used to store application sepcific
        /// data in the current session.
        /// </summary>
        public static Store Session
        {
            get
            {
                if (_sessionStorage == null)
                {
                    lock (SessionStorageLock)
                    {
                        if (_sessionStorage == null)
                            _sessionStorage = new SessionStorage();
                    }
                }
                return _sessionStorage;
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Gets the data stored with the specified key in <see cref="Store"/>.
        /// </summary>
        /// <typeparam name="T">The type of data to get.</typeparam>
        /// <param name="key">object. The key that uniquely identifies the data to retrieve.</param>
        /// <returns>A <typeparamref name="T"/> instance or null if not found.</returns>
        public T Get<T>(object key)
        {
            if (UseLocking)
            {
                lock (LockInstance)
                    return (T) GetInternalHashtable()[key];
            }
            return (T) GetInternalHashtable()[key];
        }

        /// <summary>
        /// Adds or updates the data specified with the key in <see cref="Store"/>.
        /// </summary>
        /// <typeparam name="T">The type of data to add or update.</typeparam>
        /// <param name="key">object. The key that uniquely identifies the data being stored.</param>
        /// <param name="value"><typeparamref name="T"/>. The value to add or update.</param>
        public void Set<T>(object key, T value)
        {
            if (UseLocking)
            {
                lock (LockInstance)
                    GetInternalHashtable()[key] = value;
            }
            else
                GetInternalHashtable()[key] = value;
        }

        /// <summary>
        /// Checks whether the <see cref="AppStorage"/> contains data with the specified key.
        /// </summary>
        /// <param name="key">object. The unique key to check for.</param>
        /// <returns>True if data specified with the key was found, else false.</returns>
        public bool Contains(object key)
        {
            if (UseLocking)
            {
                lock (LockInstance)
                    return GetInternalHashtable().ContainsKey(key);
            }
            return GetInternalHashtable().ContainsKey(key);
        }

        /// <summary>
        /// Removes the data specified with the key in <see cref="Store"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void Remove(object key)
        {
            if (UseLocking)
            {
                lock (LockInstance)
                    GetInternalHashtable().Remove(key);
            }
            else
                GetInternalHashtable().Remove(key);
        }

        /// <summary>
        /// Removes all data in <see cref="Store"/>.
        /// </summary>
        public void Clear()
        {
            if (UseLocking)
            {
                lock (LockInstance)
                    GetInternalHashtable().Clear();
            }
            else
                GetInternalHashtable().Clear();
        }

        #endregion

        #region abstract members

        /// <summary>
        /// When overriden, tells whether the <see cref="Store"/> uses locking when retrieving and setting values.
        /// </summary>
        protected abstract bool UseLocking { get; }

        /// <summary>
        /// Gets the object used by the <see cref="Store"/> for locking when retrieving and setting values.
        /// </summary>
        protected abstract object LockInstance { get; }

        ///<summary>
        /// When overriden by a sub class, provides the internal Hashtable used to store and retrieve
        /// data.
        ///</summary>
        ///<returns></returns>
        protected abstract Hashtable GetInternalHashtable();

        #endregion
    }
}