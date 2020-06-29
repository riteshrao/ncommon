using Microsoft.Practices.Unity;
using NCommon.Configuration;
using NCommon.ContainerAdapter.Unity;
using NCommon;
using NCommon.StateStorage;
using NUnit.Framework;

namespace NCommon.ContainerAdapters.Tests.Unity
{
    [TestFixture]
    public class when_configuring_default_state
    {
        IUnityContainer _container;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _container = new UnityContainer();
            ConfigureNCommon
                .Using(new UnityContainerAdapter(_container))
                .ConfigureState<DefaultStateConfiguration>();
        }

        [Test]
        public void verify_can_get_instance_of_IContext()
        {
            var context = _container.Resolve<IContext>();
            Assert.That(context, Is.Not.Null);
            Assert.That(context, Is.TypeOf<Context.Impl.Context>());
        }

        [Test]
        public void verify_instance_of_IApplicationState_is_ApplicationState()
        {
            var appState = _container.Resolve<IApplicationState>();
            Assert.That(appState, Is.Not.Null);
            Assert.That(appState, Is.TypeOf<ApplicationState>());
        }

        [Test]
        public void verify_instance_of_ICacheState_is_HttpRuntimeCache()
        {
            var appState = _container.Resolve<ICacheState>();
            Assert.That(appState, Is.Not.Null);
            Assert.That(appState, Is.TypeOf<HttpRuntimeCache>());
        }

        [Test]
        public void verify_instance_of_ILocalStateSelector_is_DefaultLocalStateSelector()
        {
            var localStateSelector = _container.Resolve<ILocalStateSelector>();
            Assert.That(localStateSelector, Is.Not.Null);
            Assert.That(localStateSelector, Is.TypeOf<DefaultLocalStateSelector>());
        }

        [Test]
        public void verify_instance_of_ILocalState_is_LocalStateWrapper()
        {
            var stateWrapper = _container.Resolve<ILocalState>();
            Assert.That(stateWrapper, Is.Not.Null);
            Assert.That(stateWrapper, Is.TypeOf<LocalStateWrapper>());
        }

        [Test]
        public void verify_instance_of_ISessionStateSelector_is_DefaultSessionStateSelector()
        {
            var selector = _container.Resolve<ISessionStateSelector>();
            Assert.That(selector, Is.Not.Null);
            Assert.That(selector, Is.TypeOf<DefaultSessionStateSelector>());
        }

        [Test]
        public void verify_instance_of_ISessionState_is_SessionStateWrapper()
        {
            var stateWrapper = _container.Resolve<ISessionState>();
            Assert.That(stateWrapper, Is.Not.Null);
            Assert.That(stateWrapper, Is.TypeOf<SessionStateWrapper>());
        }

        [Test]
        public void verify_instance_of_IState_is_State()
        {
            var state = _container.Resolve<IState>();
            Assert.That(state, Is.Not.Null);
            Assert.That(state, Is.TypeOf<State.Impl.State>());
        }
    }
}