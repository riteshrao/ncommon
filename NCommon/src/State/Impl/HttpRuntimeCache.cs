using System;
using System.Web;

namespace NCommon.State.Impl
{
    public class HttpRuntimeCache : ICacheState
    {
        public T Get<T>(object key)
        {
            Guard.Against<ArgumentNullException>(key == null,
                                                 "Expected a non-null key identifying the " + typeof(T).FullName +
                                                 " instance to retrieve.");
            var fullKey = typeof (T).FullName + key;
            return (T) HttpRuntime.Cache[fullKey];
        }

        public void Put<T>(object key, T instance)
        {
            Guard.Against<ArgumentNullException>(key == null,
                                                 "Expected a non-null key identifying the " + typeof(T).FullName +
                                                 " instance to put.");
            var fullKey = typeof (T).FullName + key;
            HttpRuntime.Cache.Insert(fullKey, instance);
        }

        public void Put<T>(object key, T instance, DateTime absoluteExpiration)
        {
            Guard.Against<ArgumentNullException>(key == null,
                                                 "Expected a non-null key identifying the " + typeof(T).FullName +
                                                 " instance to put.");
            var fullKey = typeof (T).FullName + key;
            HttpRuntime.Cache.Insert(fullKey, instance, null, absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration);
        }

        public void Put<T>(object key, T instance, TimeSpan slidingExpiration)
        {
            Guard.Against<ArgumentNullException>(key == null,
                                                 "Expected a non-null key identifying the " + typeof(T).FullName +
                                                 " instance to put.");
            var fullKey = typeof (T).FullName + key;
            HttpRuntime.Cache.Insert(fullKey, instance, null, System.Web.Caching.Cache.NoAbsoluteExpiration, slidingExpiration);
        }

        public void Remove<T>(object key)
        {
            Guard.Against<ArgumentNullException>(key == null,
                                                 "Expected a non-null key identifying the " + typeof(T).FullName +
                                                 " instance to remove.");
            var fullKey = typeof (T).FullName + key;
            HttpRuntime.Cache.Remove(fullKey);
        }
    }
}