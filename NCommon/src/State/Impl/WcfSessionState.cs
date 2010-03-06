using System;
using System.Collections;
using System.ServiceModel;
using NCommon.Context;

namespace NCommon.State.Impl
{
    public class WcfSessionState : ISessionState
    {
        public class WcfSessionStatExtension : IExtension<InstanceContext>
        {
            readonly Hashtable _state = new Hashtable();

            public void Add(string key, object instance)
            {
                lock(_state.SyncRoot)
                    _state.Add(key, instance);
            }
            public object Get(string key)
            {
                lock (_state.SyncRoot)
                    return _state[key];
            }
            public void Remove(string key)
            {
                lock(_state.SyncRoot)
                    _state.Remove(key);
            }

            public void Attach(InstanceContext owner) {}
            public void Detach(InstanceContext owner) {}
        }

        readonly WcfSessionStatExtension _state;

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

        public T Get<T>(object key)
        {
            var fullKey = typeof (T).FullName + key;
            return (T) _state.Get(fullKey);
        }

        public void Put<T>(object key, T instance)
        {
            var fullKey = typeof(T).FullName + key;
            _state.Add(fullKey, instance);
        }

        public void Remove<T>(object key)
        {
            var fullKey = typeof(T).FullName + key;
            _state.Remove(fullKey);
        }
    }
}