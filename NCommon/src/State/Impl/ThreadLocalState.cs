using System;
using System.Collections;

namespace NCommon.State.Impl
{
    public class ThreadLocalState : ILocalState
    {
        [ThreadStatic]
        static Hashtable _state;

        public ThreadLocalState()
        {
            if (_state == null)
                _state = new Hashtable();
        }

        public T Get<T>(object key)
        {
            var fullKey = typeof (T).FullName + key;
            return (T) _state[fullKey];
        }

        public void Put<T>(object key, T instance)
        {
            var fullKey = typeof (T).FullName + key;
            _state[fullKey] = instance;
        }

        public void Remove<T>(object key)
        {
            var fullKey = typeof (T).FullName + key;
            _state.Remove(fullKey);
        }
    }
}