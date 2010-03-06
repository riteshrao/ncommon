using System;
using System.Threading;
using System.Web;
using NCommon.State.Impl;
using NUnit.Framework;

namespace NCommon.Tests.State
{
    [TestFixture]
    public class HtpRuntimeCacheTests
    {
        [Test]
        public void Put_adds_state_to_runtime_cache()
        {
            var state = "test";
            var httpCache = new HttpRuntimeCache();
            httpCache.Put("test_key", "test");

            var key = typeof (string).FullName + "test_key";
            var returned = HttpRuntime.Cache.Get(key);
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned, Is.EqualTo(state));
            HttpRuntime.Cache.Remove(key);
        }

        [Test]
        public void Get_retrieves_state_from_runtime_cache()
        {
            var state = "test";
            var httpCache = new HttpRuntimeCache();
            httpCache.Put("test_key", "test");
            var returned = httpCache.Get<string>("test_key");
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned, Is.EqualTo(state));
            HttpRuntime.Cache.Remove(typeof (string).FullName + "test_key");
        }

        [Test]
        public void Remove_removes_state_from_runtime_cache()
        {
            var state = "test";
            var httpCache = new HttpRuntimeCache();
            httpCache.Put("test_key", state);
            Assert.That(httpCache.Get<string>("test_key"), Is.EqualTo(state));
            httpCache.Remove<string>("test_key");
            Assert.That(httpCache.Get<string>("test_key"), Is.Null);
        }

        [Test]
        public void Put_with_absolute_expiration_removes_key_after_expiration_period()
        {
            var state = "test";
            var httpCache = new HttpRuntimeCache();
            httpCache.Put("test_key", state, DateTime.Now.AddMilliseconds(500));
            Thread.Sleep(TimeSpan.FromMilliseconds(800));
            Assert.That(httpCache.Get<string>("test_key"), Is.Null);
        }

        [Test]
        public void Put_with_sliding_expiration_removes_key_after_expiration_period()
        {
            var state = "test";
            var httpCache = new HttpRuntimeCache();
            httpCache.Put("test_key", state, TimeSpan.FromMilliseconds(500));
            Thread.Sleep(TimeSpan.FromMilliseconds(200));
            Assert.That(httpCache.Get<string>("test_key"), Is.Not.Null);
            Thread.Sleep(TimeSpan.FromMilliseconds(800));
            Assert.That(httpCache.Get<string>("test_key"), Is.Null);
        }
    }
}