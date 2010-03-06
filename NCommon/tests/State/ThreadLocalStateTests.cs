using System;
using System.Threading;
using NCommon.State.Impl;
using NUnit.Framework;

namespace NCommon.Tests.State
{
    [TestFixture]
    public class ThreadLocalStateTests
    {
        [Test]
        public void can_put_and_get()
        {
            using (var tests = new CrossThreadTests())
            {
                tests.Test(() =>
                {
                    var data = "test";
                    var state = new ThreadLocalState();
                    state.Put("test_key", data);
                    Assert.That(state.Get<string>("test_key"), Is.EqualTo("test"));
                });
            }
        }

        [Test]
        public void can_remove()
        {
            using (var tests = new CrossThreadTests())
            {
                tests.Test(() =>
                {
                    var data = "test";
                    var state = new ThreadLocalState();
                    state.Put("test_key", data);
                    Assert.That(state.Get<string>("test_key"), Is.EqualTo("test"));
                    state.Remove<string>("test_key");
                    Assert.That(state.Get<string>("test_key"), Is.Null);
                });
            }
        }
    }
}