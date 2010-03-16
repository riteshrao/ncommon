using System;
using NCommon.State;

namespace NCommon.ContainerAdapters.Tests
{
    public class FakeApplicationState : IApplicationState
    {
        public T Get<T>(object key) { return default(T); }

        public void Put<T>(object key, T instance) { }

        public void Remove<T>(object key) { }
    }

    public class FakeLocalState : ILocalState
    {
        public T Get<T>(object key) { return default(T); }

        public void Put<T>(object key, T instance) { }

        public void Remove<T>(object key) { }
    }

    public class FakeSessionState : ISessionState
    {
        public T Get<T>(object key) { return default(T); }

        public void Put<T>(object key, T instance) { }

        public void Remove<T>(object key) { }
    }

    public class FakeCacheState : ICacheState
    {
        public T Get<T>(object key) { return default(T); }

        public void Put<T>(object key, T instance) { }

        public void Put<T>(object key, T instance, DateTime absoluteExpiration) {}

        public void Put<T>(object key, T instance, TimeSpan slidingExpiration) {}

        public void Remove<T>(object key) { }
    }
}