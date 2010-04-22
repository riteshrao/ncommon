using System;
using System.Collections;
using NCommon.State;

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

        public IApplicationState Application
        {
            get { return _app; }
        }

        public ISessionState Session
        {
            get { return _session; }
        }

        public ICacheState Cache
        {
            get { return _cache; }
        }

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

            public void Put<T>(object key, T instance, DateTime absoluteExpiration)
            {
                Put(key, instance);
            }

            public void Put<T>(object key, T instance, TimeSpan slidingExpiration)
            {
                Put(key, instance);
            }

            public void Remove<T>(object key)
            {
                _state.Remove(typeof(T).FullName + key);
            }
        }
    }
}