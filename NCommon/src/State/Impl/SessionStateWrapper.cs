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

namespace NCommon.State.Impl
{
    /// <summary>
    /// Implementation of <see cref="ISessionState"/> that wraps a <see cref="ISessionState"/> instance
    /// from a <see cref="ISessionStateSelector"/>.
    /// </summary>
    public class SessionStateWrapper : ISessionState
    {
        readonly ISessionState _state;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="SessionStateWrapper"/> class.
        /// </summary>
        /// <param name="selector">A <see cref="ISessionStateSelector"/> instance used to retrieve the underlying
        /// <see cref="ISessionState"/> instance.</param>
        public SessionStateWrapper(ISessionStateSelector selector)
        {
            _state = selector.Get();
        }

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
            return _state.Get<T>(key);
        }

        /// <summary>
        /// Puts state data into the session state with the default key.
        /// </summary>
        /// <typeparam name="T">The type of data to put.</typeparam>
        /// <param name="instance">An instance of <typeparamref name="T"/> to store.</param>
        public void Put<T>(T instance)
        {
            Put(null, instance);
        }

        /// <summary>
        /// Puts state data into the session state with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of data to put.</typeparam>
        /// <param name="key">An object representing the unique key with which the data is stored.</param>
        /// <param name="instance">An instance of <typeparamref name="T"/> to store.</param>
        public void Put<T>(object key, T instance)
        {
            _state.Put(key, instance);
        }

        /// <summary>
        /// Removes state data stored in the session state with the default key.
        /// </summary>
        /// <typeparam name="T">The type of data to remove.</typeparam>
        public void Remove<T>()
        {
            Remove<T>(null);
        }

        /// <summary>
        /// Removes state data stored in the session state with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of data to remove.</typeparam>
        /// <param name="key">An object representing the unique key with which the data was stored.</param>
        public void Remove<T>(object key)
        {
            _state.Remove<T>(key);
        }

        /// <summary>
        /// Clears all state data stored in the session.
        /// </summary>
        public void Clear()
        {
            _state.Clear();
        }
    }
}