using System;
using System.Threading;
using System.Web;
using NCommon.StateStorage;
using NUnit.Framework;

namespace NCommon.Tests.State
{
    [TestFixture]
    public class HtpRuntimeCacheTests
    {
        [TearDown]
        public void Test_TearDown()
        {
            HttpRuntime.Cache.Remove(Utils.BuildFullKey<string>(null));
            HttpRuntime.Cache.Remove(Utils.BuildFullKey<string>("test_key"));
        }

        [Test]
        public void Can_put()
        {
            var state = "test";
            var httpCache = new HttpRuntimeCache();
            httpCache.Put("test_key", "test");

            var key = Utils.BuildFullKey<string>("test_key");
            var returned = HttpRuntime.Cache.Get(key);
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned, Is.EqualTo(state));
            HttpRuntime.Cache.Remove(key);
        }

        [Test]
        public void Can_put_using_default_key()
        {
            var state = "test";
            var httpCache = new HttpRuntimeCache();
            httpCache.Put(null, state);

            var key = Utils.BuildFullKey<string>(null);
            var returned = HttpRuntime.Cache.Get(key);
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned, Is.EqualTo(state));
        }

        [Test]
        public void Can_get()
        {
            var state = "test";
            var httpCache = new HttpRuntimeCache();
            httpCache.Put("test_key", "test");
            var returned = httpCache.Get<string>("test_key");
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned, Is.EqualTo(state));
        }

        [Test]
        public void Can_get_using_default_key()
        {
            var state = "test";
            var httpCache = new HttpRuntimeCache();
            httpCache.Put(state);

            var returned = httpCache.Get<string>();
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned, Is.EqualTo(state));
        }

        [Test]
        public void Can_remove()
        {
            var state = "test";
            var httpCache = new HttpRuntimeCache();
            httpCache.Put("test_key", state);
            Assert.That(httpCache.Get<string>("test_key"), Is.EqualTo(state));
            httpCache.Remove<string>("test_key");
            Assert.That(httpCache.Get<string>("test_key"), Is.Null);
        }

        [Test]
        public void Can_remove_state_with_default_key()
        {
            var state = "test";
            var httpCache = new HttpRuntimeCache();
            httpCache.Put(state);
            Assert.That(httpCache.Get<string>(), Is.EqualTo(state));
            httpCache.Remove<string>();
            Assert.That(httpCache.Get<string>(), Is.Null);
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