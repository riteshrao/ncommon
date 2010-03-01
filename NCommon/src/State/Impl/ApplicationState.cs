using System;
using System.Collections;

namespace NCommon.State.Impl
{
    ///<summary>
    /// Default implementation of <see cref="IApplicationState"/>
    ///</summary>
    public class ApplicationState : IApplicationState
    {
        static readonly Hashtable _applicationState = new Hashtable();

        public T Get<T>(object key)
        {
            Guard.Against<ArgumentNullException>(key == null,
                                                 "Expected a non-null key identifying the " + typeof (T).FullName +
                                                 " instance to retrieve.");
            var fullKey = typeof (T).FullName + key;
            return (T) _applicationState[fullKey];
        }

        public void Put<T>(object key, T instance)
        {
            Guard.Against<ArgumentNullException>(key == null,
                                                 "Expected a non-null key identifying the " + typeof(T).FullName +
                                                 " instance to retrieve.");
            var fullKey = typeof (T).FullName + key;
            _applicationState[fullKey] = instance;
        }

        public void Remove<T>(object key)
        {
            Guard.Against<ArgumentNullException>(key == null,
                                                 "Expected a non-null key identifying the " + typeof(T).FullName +
                                                 " instance to retrieve.");
            var fullKey = typeof (T).FullName + key;
            _applicationState.Remove(fullKey);
        }
    }
}