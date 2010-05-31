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
using System.ServiceModel;
using NCommon.Context;

namespace NCommon.State.Impl
{
    /// <summary>
    /// Implementation of <see cref="ISessionState"/> that stores session data in the current wcf session.
    /// </summary>
    public class WcfSessionState : ISessionState
    {
        /// <summary>
        /// Implementation of <see cref="IExtension{T}"/> of type <see cref="InstanceContext"/> that stores
        /// session state data in the current <see cref="InstanceContext"/>.
        /// </summary>
        class WcfSessionStatExtension : IExtension<InstanceContext>
        {
            readonly Hashtable _state = new Hashtable();
            /// <summary>
            /// Adds state data with the given key.
            /// </summary>
            /// <param name="key">string. The unique key.</param>
            /// <param name="instance">object. The state data to store.</param>
            public void Add(string key, object instance)
            {
                lock(_state.SyncRoot)
                    _state.Add(key, instance);
            }

            /// <summary>
            /// Gets state data stored with the specified unique key.
            /// </summary>
            /// <param name="key">string. The unique key.</param>
            /// <returns>object. A non-null reference if the data is found, else null.</returns>
            public object Get(string key)
            {
                lock (_state.SyncRoot)
                    return _state[key];
            }

            /// <summary>
            /// Removes state data stored with the specified unique key.
            /// </summary>
            /// <param name="key">string. The unique key.</param>
            public void Remove(string key)
            {
                lock(_state.SyncRoot)
                    _state.Remove(key);
            }

            /// <summary>
            /// Clears all state data.
            /// </summary>
            public void Clear()
            {
                lock(_state.SyncRoot)
                    _state.Clear();
            }

            public void Attach(InstanceContext owner) {}

            public void Detach(InstanceContext owner) {}
        }

        readonly WcfSessionStatExtension _state;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of <see cref="WcfSessionState"/> class.
        /// </summary>
        /// <param name="context">An instance of <see cref="IContext"/>.</param>
        public WcfSessionState(IContext context)
        {
            _state = context.OperationContext.InstanceContext.Extensions.Find<WcfSessionStatExtension>();
            if (_state == null)
            {
                lock(context.OperationContext.InstanceContext.SynchronizationContext)
                {
                    _state = context.OperationContext.InstanceContext.Extensions.Find<WcfSessionStatExtension>();
                    if (_state == null)
                    {
                        _state = new WcfSessionStatExtension();
                        context.OperationContext.InstanceContext.Extensions.Add(_state);
                    }
                }
            }
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
            return (T) _state.Get(key.BuildFullKey<T>());
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
            _state.Add(key.BuildFullKey<T>(), instance);
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
            _state.Remove(key.BuildFullKey<T>());
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