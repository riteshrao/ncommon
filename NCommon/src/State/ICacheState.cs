using System;

namespace NCommon.State
{
    ///<summary>
    /// Base interface implemented by cache state providers.
    ///</summary>
    public interface ICacheState
    {
        T Get<T>(object key);
        void Put<T>(object key, T instance);
        void Put<T>(object key, T instance, DateTime absoluteExpiration);
        void Put<T>(object key, T instance, TimeSpan slidingExpiration);
        void Remove<T>(object key);
    }
}