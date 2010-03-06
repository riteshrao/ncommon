using System;

namespace NCommon.State.Impl
{
    public class LocalStateWrapper : ILocalState
    {
        readonly ILocalState _state;

        public LocalStateWrapper(ILocalStateSelector selector)
        {
            _state = selector.Get();
        }

        public T Get<T>(object key)
        {
            Guard.Against<ArgumentNullException>(key == null,
                                                 "Expected a non-null key identifying the " + typeof(T).FullName +
                                                 " instance to retrieve.");
            return _state.Get<T>(key);
        }

        public void Put<T>(object key, T instance)
        {
            Guard.Against<ArgumentNullException>(key == null,
                                                 "Expected a non-null key identifying the " + typeof(T).FullName +
                                                 " instance to store.");
            _state.Put(key, instance);
        }

        public void Remove<T>(object key)
        {
            Guard.Against<ArgumentNullException>(key == null,
                                                 "Expected a non-null key identifying the " + typeof(T).FullName +
                                                 " instance to remove.");
            _state.Remove<T>(key);
        }
    }
}