using NCommon.Configuration;
using NCommon.ContainerAdapter.StructureMap;
using NCommon.StateStorage;
using NUnit.Framework;
using StructureMap;

namespace NCommon.ContainerAdapters.Tests.StructureMap
{
    [TestFixture]
    public class when_configuring_custom_state
    {
        IContainer _container;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _container = new Container();
            ConfigureNCommon
                .Using(new StructureMapContainerAdapter(_container))
                .ConfigureState<DefaultStateConfiguration>(config => config
                                                                         .UseCustomApplicationStateOf
                                                                         <FakeApplicationState>()
                                                                         .UseCustomLocalStateOf<FakeLocalState>()
                                                                         .UseCustomSessionStateOf<FakeSessionState>()
                                                                         .UseCustomCacheOf<FakeCacheState>());
        }

        [Test]
        public void verify_can_get_instance_of_IContext()
        {
            var context = _container.GetInstance<IContext>();
            Assert.That(context, Is.Not.Null);
            Assert.That(context, Is.TypeOf<Context>());
        }

        [Test]
        public void verify_instance_of_IApplicationState_is_FakeApplicationState()
        {
            var appState = _container.GetInstance<IApplicationState>();
            Assert.That(appState, Is.Not.Null);
            Assert.That(appState, Is.TypeOf<FakeApplicationState>());
        }

        [Test]
        public void verify_instance_of_ICacheState_is_FakeCacheState()
        {
            var appState = _container.GetInstance<ICacheState>();
            Assert.That(appState, Is.Not.Null);
            Assert.That(appState, Is.TypeOf<FakeCacheState>());
        }

        [Test]
        public void verify_instance_of_ILocalState_is_FakeLocalState()
        {
            var stateWrapper = _container.GetInstance<ILocalState>();
            Assert.That(stateWrapper, Is.Not.Null);
            Assert.That(stateWrapper, Is.TypeOf<FakeLocalState>());
        }

        [Test]
        public void verify_instance_of_ISessionState_is_FakeSessionState()
        {
            var stateWrapper = _container.GetInstance<ISessionState>();
            Assert.That(stateWrapper, Is.Not.Null);
            Assert.That(stateWrapper, Is.TypeOf<FakeSessionState>());
        }

        [Test]
        public void verify_instance_of_IState_is_State()
        {
            var state = _container.GetInstance<IState>();
            Assert.That(state, Is.Not.Null);
            Assert.That(state, Is.TypeOf<State>());
        }
    }
}