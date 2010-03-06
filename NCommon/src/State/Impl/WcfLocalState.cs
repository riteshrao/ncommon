using System.Collections;
using System.ServiceModel;
using NCommon.Context;

namespace NCommon.State.Impl
{
    public class WcfLocalState : ILocalState
    {
        readonly WcfLocalStateExtension _state;

        public class WcfLocalStateExtension : IExtension<OperationContext>
        {
            readonly Hashtable _state = new Hashtable();
            
            public void Add(string key, object instance)
            {
                _state.Add(key, instance);
            }

            public object Get(string key)
            {
                return _state[key];
            }

            public void Remove(string key)
            {
                _state.Remove(key);
            }

            public void Attach(OperationContext owner){}
            public void Detach(OperationContext owner){}
        }

        public WcfLocalState(IContext context)
        {
            _state = context.OperationContext.Extensions.Find<WcfLocalStateExtension>();
            if (_state == null)
            {
                _state = new WcfLocalStateExtension();
                context.OperationContext.Extensions.Add(_state);
            }
        }

        public T Get<T>(object key)
        {
            var fullKey = typeof (T).FullName + key;
            return (T) _state.Get(fullKey);
        }

        public void Put<T>(object key, T instance)
        {
            var fullKey = typeof (T).FullName + key;
            _state.Add(fullKey, instance);
        }

        public void Remove<T>(object key)
        {
            var fullKey = typeof (T).FullName + key;
            _state.Remove(fullKey);
        }
    }
}