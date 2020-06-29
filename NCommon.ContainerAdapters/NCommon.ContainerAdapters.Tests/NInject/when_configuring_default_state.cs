using NCommon.Configuration;
using NCommon.ContainerAdapter.Ninject;
using NCommon;
using NCommon.StateStorage;
using NCommon.StateStorage;
using Ninject;
using NUnit.Framework;

namespace NCommon.ContainerAdapters.Tests.Ninject
{
    [TestFixture]
    public class when_configuring_default_state
    {
        IKernel _kernel;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _kernel = new StandardKernel();
            ConfigureNCommon
                .Using(new NinjectContainerAdapter(_kernel))
                .ConfigureState<DefaultStateConfiguration>();
        }

        [Test]
        public void verify_can_get_instance_of_IContext()
        {
            var context = _kernel.Get<IContext>();
            Assert.That(context, Is.Not.Null);
            Assert.That(context, Is.TypeOf<Context.Impl.Context>());
        }

        [Test]
        public void verify_instance_of_IApplicationState_is_ApplicationState()
        {
            var appState = _kernel.Get<IApplicationState>();
            Assert.That(appState, Is.Not.Null);
            Assert.That(appState, Is.TypeOf<ApplicationState>());
        }

        [Test]
        public void verify_instance_of_ICacheState_is_HttpRuntimeCache()
        {
            var appState = _kernel.Get<ICacheState>();
            Assert.That(appState, Is.Not.Null);
            Assert.That(appState, Is.TypeOf<HttpRuntimeCache>());
        }

        [Test]
        public void verify_instance_of_ILocalStateSelector_is_DefaultLocalStateSelector()
        {
            var localStateSelector = _kernel.Get<ILocalStateSelector>();
            Assert.That(localStateSelector, Is.Not.Null);
            Assert.That(localStateSelector, Is.TypeOf<DefaultLocalStateSelector>());
        }

        [Test]
        public void verify_instance_of_ILocalState_is_LocalStateWrapper()
        {
            var stateWrapper = _kernel.Get<ILocalState>();
            Assert.That(stateWrapper, Is.Not.Null);
            Assert.That(stateWrapper, Is.TypeOf<LocalStateWrapper>());
        }

        [Test]
        public void verify_instance_of_ISessionStateSelector_is_DefaultSessionStateSelector()
        {
            var selector = _kernel.Get<ISessionStateSelector>();
            Assert.That(selector, Is.Not.Null);
            Assert.That(selector, Is.TypeOf<DefaultSessionStateSelector>());
        }

        [Test]
        public void verify_instance_of_ISessionState_is_SessionStateWrapper()
        {
            var stateWrapper = _kernel.Get<ISessionState>();
            Assert.That(stateWrapper, Is.Not.Null);
            Assert.That(stateWrapper, Is.TypeOf<SessionStateWrapper>());
        }

        [Test]
        public void verify_instance_of_IState_is_State()
        {
            var state = _kernel.Get<IState>();
            Assert.That(state, Is.Not.Null);
            Assert.That(state, Is.TypeOf<State>());
        }
    }
}