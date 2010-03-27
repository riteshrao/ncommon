using NCommon.State.Impl;
using NUnit.Framework;

namespace NCommon.Tests.State
{
    [TestFixture]
    public class ApplicationStateTests
    {
        [Test]
        public void can_put_and_get_state()
        {
            var state = "test";
            var appState = new ApplicationState();
            appState.Put("test_state", state);

            var returned = appState.Get<string>("test_state");
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned, Is.EqualTo(state));
        }

        [Test]
        public void can_remove_state()
        {
            var state = "test";
            var appState = new ApplicationState();
            appState.Put("test_state", state);

            Assert.That(appState.Get<string>("test_state"), Is.EqualTo(state));
            appState.Remove<string>("test_state");
            Assert.That(appState.Get<string>("test_state"), Is.Null);
        }
    }
}