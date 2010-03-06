using System;
using System.Collections;
using System.Web;
using NCommon.Context;

namespace NCommon.State.Impl
{
    public class HttpLocalState : ILocalState
    {
        readonly Hashtable _state;

        public HttpLocalState(IContext context)
        {
            _state = context.HttpContext.Items[typeof (HttpLocalState).FullName] as Hashtable;
            if (_state == null)
                context.HttpContext.Items[typeof(HttpLocalState).FullName] = (_state = new Hashtable());
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
            var fullKey = typeof(T).FullName + key;
            _state.Remove(fullKey);
        }
    }
}