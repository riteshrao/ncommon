using NCommon.Configuration;
using NCommon.ContainerAdapter.Ninject;
using NCommon.Context;
using NCommon.State;
using Ninject;
using NUnit.Framework;

namespace NCommon.ContainerAdapters.Tests.Ninject
{
    [TestFixture]
    public class when_configuring_custom_state
    {
        IKernel _kernel;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _kernel = new StandardKernel();
            Configure
                .Using(new NinjectContainerAdapter(_kernel))
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
            var context = _kernel.Get<IContext>();
            Assert.That(context, Is.Not.Null);
            Assert.That(context, Is.TypeOf<Context.Impl.Context>());
        }

        [Test]
        public void verify_instance_of_IApplicationState_is_FakeApplicationState()
        {
            var appState = _kernel.Get<IApplicationState>();
            Assert.That(appState, Is.Not.Null);
            Assert.That(appState, Is.TypeOf<FakeApplicationState>());
        }

        [Test]
        public void verify_instance_of_ICacheState_is_FakeCacheState()
        {
            var appState = _kernel.Get<ICacheState>();
            Assert.That(appState, Is.Not.Null);
            Assert.That(appState, Is.TypeOf<FakeCacheState>());
        }

        [Test]
        public void verify_instance_of_ILocalState_is_FakeLocalState()
        {
            var stateWrapper = _kernel.Get<ILocalState>();
            Assert.That(stateWrapper, Is.Not.Null);
            Assert.That(stateWrapper, Is.TypeOf<FakeLocalState>());
        }

        [Test]
        public void verify_instance_of_ISessionState_is_FakeSessionState()
        {
            var stateWrapper = _kernel.Get<ISessionState>();
            Assert.That(stateWrapper, Is.Not.Null);
            Assert.That(stateWrapper, Is.TypeOf<FakeSessionState>());
        }

        [Test]
        public void verify_instance_of_IState_is_State()
        {
            var state = _kernel.Get<IState>();
            Assert.That(state, Is.Not.Null);
            Assert.That(state, Is.TypeOf<State.Impl.State>());
        }
    }
}