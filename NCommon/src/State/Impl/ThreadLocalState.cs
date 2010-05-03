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
    /// <summary>
    /// Implementation of <see cref="ThreadLocalState"/> that stores local state data for the current thread.
    /// </summary>
    public class ThreadLocalState : ILocalState
    {
        [ThreadStatic]
        static Hashtable _state;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="ThreadLocalState"/> class.
        /// </summary>
        public ThreadLocalState()
        {
            if (_state == null)
                _state = new Hashtable();
        }

        /// <summary>
        /// Gets state data stored with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of data to retrieve.</typeparam>
        /// <param name="key">An object representing the unique key with which the data was stored.</param>
        /// <returns>An instance of <typeparamref name="T"/> or null if not found.</returns>
        public T Get<T>(object key)
        {
            var fullKey = typeof (T).FullName + key;
            return (T) _state[fullKey];
        }

        /// <summary>
        /// Puts state data into the local state with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of data to put.</typeparam>
        /// <param name="key">An object representing the unique key with which the data is stored.</param>
        /// <param name="instance">An instance of <typeparamref name="T"/> to store.</param>
        public void Put<T>(object key, T instance)
        {
            var fullKey = typeof (T).FullName + key;
            _state[fullKey] = instance;
        }

        /// <summary>
        /// Removes state data stored in the local state with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of data to remove.</typeparam>
        /// <param name="key">An object representing the unique key with which the data was stored.</param>
        public void Remove<T>(object key)
        {
            var fullKey = typeof (T).FullName + key;
            _state.Remove(fullKey);
        }
    }
}