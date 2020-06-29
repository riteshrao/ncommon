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

using System;
using System.Collections;
using NCommon.StateStorage;

namespace NCommon.Testing
{
    ///<summary>
    /// A fake <see cref="IState"/> implementation.
    ///</summary>
    public class FakeState : IState
    {
        readonly State _app = new State();
        readonly State _local = new State();
        readonly State _session = new State();
        readonly State _cache = new State();

        /// <summary>
        /// Gets the application specific state.
        /// </summary>
        public IApplicationState Application
        {
            get { return _app; }
        }

        /// <summary>
        /// Gets the session specific state.
        /// </summary>
        public ISessionState Session
        {
            get { return _session; }
        }

        /// <summary>
        /// Gets the cache specific state.
        /// </summary>
        public ICacheState Cache
        {
            get { return _cache; }
        }

        /// <summary>
        /// Gets the thread local / request local specific state.
        /// </summary>
        public ILocalState Local
        {
            get { return _local; }
        }

        class State : IApplicationState, ILocalState, ISessionState, ICacheState
        {
            readonly Hashtable _state = new Hashtable();

            public T Get<T>(object key)
            {
                return (T)_state[typeof(T).FullName + key];
            }

            public void Put<T>(object key, T instance)
            {
                _state[typeof(T).FullName + key] = instance;
            }

            public void Put<T>(T instance, DateTime absoluteExpiration)
            {
                Put(instance);
            }

            public void Put<T>(object key, T instance, DateTime absoluteExpiration)
            {
                Put(key, instance);
            }

            public void Put<T>(T instance, TimeSpan slidingExpiration)
            {
                Put(instance);
            }

            public void Put<T>(object key, T instance, TimeSpan slidingExpiration)
            {
                Put(key, instance);
            }

            public void Remove<T>(object key)
            {
                _state.Remove(typeof(T).FullName + key);
            }

            public T Get<T>()
            {
                return (T) _state[typeof (T).FullName];
            }

            public void Put<T>(T instance)
            {
                _state[typeof (T).FullName] = instance;
            }

            public void Remove<T>()
            {
                _state.Remove(typeof (T).FullName);
            }

            public void Clear()
            {
                _state.Clear();
            }
        }
    }
}