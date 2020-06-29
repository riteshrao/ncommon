using NCommon.StateStorage;
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
        public void can_put_and_get_state_with_default_key()
        {
            var state = "test";
            var appState = new ApplicationState();
            appState.Put(state);

            var returned = appState.Get<string>();
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

        [Test]
        public void can_remove_state_with_default_key()
        {
            var state = "test";
            var appState = new ApplicationState();
            appState.Put(state);

            Assert.That(appState.Get<string>(), Is.EqualTo(state));
            appState.Remove<string>();
            Assert.That(appState.Get<string>(), Is.Null);
        }

        [Test]
        public void can_clear_state()
        {
            var state = new ApplicationState();
            state.Put("DefaultState");
            state.Put("test_key", "KeyedState");

            Assert.That(state.Get<string>(), Is.EqualTo("DefaultState"));
            Assert.That(state.Get<string>("test_key"), Is.EqualTo("KeyedState"));

            state.Clear();
            Assert.That(state.Get<string>(), Is.Null);
            Assert.That(state.Get<string>("test_key"), Is.Null);
        }
    }
}