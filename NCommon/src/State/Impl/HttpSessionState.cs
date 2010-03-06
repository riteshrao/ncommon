using System;
using System.Collections;
using NCommon.Context;

namespace NCommon.State.Impl
{
    public class HttpSessionState : ISessionState
    {
        readonly Hashtable _state;

        public HttpSessionState(IContext context)
        {
            _state = context.HttpContext.Session[typeof (HttpSessionState).AssemblyQualifiedName] as Hashtable;
            if (_state == null)
            {
                lock(context.HttpContext.Session.SyncRoot)
                {
                    _state = context.HttpContext.Session[typeof(HttpSessionState).AssemblyQualifiedName] as Hashtable;
                    if (_state == null)
                        context.HttpContext.Session[typeof(HttpSessionState).AssemblyQualifiedName] = (_state = new Hashtable());
                }
            }
        }

        public T Get<T>(object key)
        {
            var fullKey = typeof (T).FullName + key;
            lock (_state.SyncRoot)
                return (T) _state[fullKey];
        }

        public void Put<T>(object key, T instance)
        {
            var fullKey = typeof (T).FullName + key;
            lock (_state.SyncRoot)
                _state[fullKey] = instance;
        }

        public void Remove<T>(object key)
        {
            var fullKey = typeof (T).FullName + key;
            lock (_state.SyncRoot)
                _state.Remove(fullKey);
        }
    }
}