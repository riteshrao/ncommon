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
    /// Implementation of <see cref="ILocalState"/> that stores local state data for a Wcf operation.
    /// </summary>
    public class WcfLocalState : ILocalState
    {
        readonly WcfLocalStateExtension _state;

        /// <summary>
        /// A custom <see cref="IExtension{T}"/> of type <see cref="OperationContext"/> that is used
        /// to stored local state for the current <see cref="OperationContext"/>.
        /// </summary>
        public class WcfLocalStateExtension : IExtension<OperationContext>
        {
            readonly Hashtable _state = new Hashtable();
            
            /// <summary>
            /// Adds state data with the given key.
            /// </summary>
            /// <param name="key">string. The unique key.</param>
            /// <param name="instance">object. The state data to store.</param>
            public void Add(string key, object instance)
            {
                _state.Add(key, instance);
            }

            /// <summary>
            /// Gets state data stored with the specified unique key.
            /// </summary>
            /// <param name="key">string. The unique key.</param>
            /// <returns>object. A non-null reference if the data is found, else null.</returns>
            public object Get(string key)
            {
                return _state[key];
            }

            /// <summary>
            /// Removes state data stored with the specified unique key.
            /// </summary>
            /// <param name="key">string. The unique key.</param>
            public void Remove(string key)
            {
                _state.Remove(key);
            }

            /// <summary>
            /// Enables an extension object to find out when it has been aggregated. Called when the extension is added to the <see cref="P:System.ServiceModel.IExtensibleObject`1.Extensions"/> property.
            /// </summary>
            /// <param name="owner">The extensible object that aggregates this extension.</param>
            public void Attach(OperationContext owner){}


            /// <summary>
            /// Enables an object to find out when it is no longer aggregated. Called when an extension is removed from the <see cref="P:System.ServiceModel.IExtensibleObject`1.Extensions"/> property.
            /// </summary>
            /// <param name="owner">The extensible object that aggregates this extension.</param>
            public void Detach(OperationContext owner){}

            ///<summary>
            /// Clears all stored state.
            ///</summary>
            public void Clear()
            {
                _state.Clear();
            }
        }

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="WcfLocalState"/> class.
        /// </summary>
        /// <param name="context">An instance of <see cref="IContext"/></param>
        public WcfLocalState(IContext context)
        {
            _state = context.OperationContext.Extensions.Find<WcfLocalStateExtension>();
            if (_state == null)
            {
                _state = new WcfLocalStateExtension();
                context.OperationContext.Extensions.Add(_state);
            }
        }

        /// <summary>
        /// Gets state data stored with the default key.
        /// </summary>
        /// <typeparam name="T">The type of data to retrieve.</typeparam>
        /// <returns>An isntance of <typeparamref name="T"/> or null if not found.</returns>
        public T Get<T>()
        {
            var fullKey = typeof (T).FullName;
            return (T) _state.Get(fullKey);
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
            return (T) _state.Get(fullKey);
        }

        /// <summary>
        /// Puts state data into the local state with the default key.
        /// </summary>
        /// <typeparam name="T">The type of data to put.</typeparam>
        /// <param name="instance">An instance of <typeparamref name="T"/> to put.</param>
        public void Put<T>(T instance)
        {
            var fullKey = typeof (T).FullName;
            _state.Add(fullKey, instance);
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
            _state.Add(fullKey, instance);
        }

        /// <summary>
        /// Removes state data stored in the local state with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of data to remove.</typeparam>
        public void Remove<T>()
        {
            var fullKey = typeof (T).FullName;
            _state.Remove(fullKey);
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

        /// <summary>
        /// Clears all state stored in local state.
        /// </summary>
        public void Clear()
        {
            _state.Clear();
        }
    }
}