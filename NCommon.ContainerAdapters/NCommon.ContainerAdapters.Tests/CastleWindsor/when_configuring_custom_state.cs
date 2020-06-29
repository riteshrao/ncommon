using Castle.Windsor;
using NCommon.Configuration;
using NCommon.ContainerAdapter.CastleWindsor;
using NCommon;
using NCommon.StateStorage;
using NUnit.Framework;

namespace NCommon.ContainerAdapters.Tests.CastleWindsor
{
    [TestFixture]
    public class when_configuring_custom_state
    {
        IWindsorContainer _container = null;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _container = new WindsorContainer();
            NCommon.ConfigureNCommon
                .Using(new WindsorContainerAdapter(_container))
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
            var context = _container.Resolve<IContext>();
            Assert.That(context, Is.Not.Null);
            Assert.That(context, Is.TypeOf<Context.Impl.Context>());
        }

        [Test]
        public void verify_instance_of_IApplicationState_is_FakeApplicationState()
        {
            var appState = _container.Resolve<IApplicationState>();
            Assert.That(appState, Is.Not.Null);
            Assert.That(appState, Is.TypeOf<FakeApplicationState>());
        }

        [Test]
        public void verify_instance_of_ICacheState_is_FakeCacheState()
        {
            var appState = _container.Resolve<ICacheState>();
            Assert.That(appState, Is.Not.Null);
            Assert.That(appState, Is.TypeOf<FakeCacheState>());
        }

        [Test]
        public void verify_instance_of_ILocalState_is_FakeLocalState()
        {
            var stateWrapper = _container.Resolve<ILocalState>();
            Assert.That(stateWrapper, Is.Not.Null);
            Assert.That(stateWrapper, Is.TypeOf<FakeLocalState>());
        }

        [Test]
        public void verify_instance_of_ISessionState_is_FakeSessionState()
        {
            var stateWrapper = _container.Resolve<ISessionState>();
            Assert.That(stateWrapper, Is.Not.Null);
            Assert.That(stateWrapper, Is.TypeOf<FakeSessionState>());
        }

        [Test]
        public void verify_instance_of_IState_is_State()
        {
            var state = _container.Resolve<IState>();
            Assert.That(state, Is.Not.Null);
            Assert.That(state, Is.TypeOf<State>());
        }
    }
}