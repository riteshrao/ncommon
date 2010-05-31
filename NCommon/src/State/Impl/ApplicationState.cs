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
using System.Collections;

namespace NCommon.State.Impl
{
    ///<summary>
    /// Default implementation of <see cref="IApplicationState"/>
    ///</summary>
    public class ApplicationState : IApplicationState
    {
        readonly static object _syncRoot = new object();
        readonly Hashtable _applicationState = new Hashtable();

        /// <summary>
        /// Gets state data stored with the default key.
        /// </summary>
        /// <typeparam name="T">The type of data to retrieve.</typeparam>
        /// <returns>An instance of <typeparamref name="T"/> or null if not found.</returns>
        public T Get<T>()
        {
            return Get<T>(null);
        }

        /// <summary>
        /// Gets state data stored with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of data to retrieve.</typeparam>
        /// <param name="key">An object representing the unique key with which the data was stored.</param>
        /// <returns>An instance of <typeparamref name="T"/> or null if not found.</returns>
        public T Get<T>(object key)
        {
            lock(_syncRoot)
                return (T) _applicationState[key.BuildFullKey<T>()];
        }

        ///<summary>
        /// Puts state data into the application state using the type's name as the default key.
        ///</summary>
        ///<param name="instance">The instance of <typeparamref name="T"/></param>
        ///<typeparam name="T"></typeparam>
        public void Put<T>(T instance)
        {
            Put(null, instance);
        }

        /// <summary>
        /// Puts state data into the application state with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of data to put.</typeparam>
        /// <param name="key">An object representing the unique key with which the data is stored.</param>
        /// <param name="instance">An instance of <typeparamref name="T"/> to store.</param>
        public void Put<T>(object key, T instance)
        {
            lock (_syncRoot)
                _applicationState[key.BuildFullKey<T>()] = instance;
        }

        /// <summary>
        /// Removes state data stored in the application state with the default key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Remove<T>()
        {
            Remove<T>(null);
        }

        /// <summary>
        /// Removes state data stored in the application state with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of data to remove.</typeparam>
        /// <param name="key">An object representing the unique key with which the data was stored.</param>
        public void Remove<T>(object key)
        {
            lock(_syncRoot)
                _applicationState.Remove(key.BuildFullKey<T>());
        }

        /// <summary>
        /// Clears all state data stored in the application state.
        /// </summary>
        public void Clear()
        {
            lock(_syncRoot)
                _applicationState.Clear();
        }
    }
}