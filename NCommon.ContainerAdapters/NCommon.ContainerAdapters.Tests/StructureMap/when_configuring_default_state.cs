using NCommon.Configuration;
using NCommon.ContainerAdapter.StructureMap;
using NCommon.StateStorage;
using NUnit.Framework;
using StructureMap;

namespace NCommon.ContainerAdapters.Tests.StructureMap
{
    [TestFixture]
    public class when_configuring_default_state
    {
        IContainer _container;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _container = new Container();
            ConfigureNCommon
                .Using(new StructureMapContainerAdapter(_container))
                .ConfigureState<DefaultStateConfiguration>();
        }

        [Test]
        public void verify_can_get_instance_of_IContext()
        {
            var context = _container.GetInstance<Context.IContext>();
            Assert.That(context, Is.Not.Null);
            Assert.That(context, Is.TypeOf<Context.Impl.Context>());
        }

        [Test]
        public void verify_instance_of_IApplicationState_is_ApplicationState()
        {
            var appState = _container.GetInstance<IApplicationState>();
            Assert.That(appState, Is.Not.Null);
            Assert.That(appState, Is.TypeOf<ApplicationState>());
        }

        [Test]
        public void verify_instance_of_ICacheState_is_HttpRuntimeCache()
        {
            var appState = _container.GetInstance<ICacheState>();
            Assert.That(appState, Is.Not.Null);
            Assert.That(appState, Is.TypeOf<HttpRuntimeCache>());
        }

        [Test]
        public void verify_instance_of_ILocalStateSelector_is_DefaultLocalStateSelector()
        {
            var localStateSelector = _container.GetInstance<ILocalStateSelector>();
            Assert.That(localStateSelector, Is.Not.Null);
            Assert.That(localStateSelector, Is.TypeOf<DefaultLocalStateSelector>());
        }

        [Test]
        public void verify_instance_of_ILocalState_is_LocalStateWrapper()
        {
            var stateWrapper = _container.GetInstance<ILocalState>();
            Assert.That(stateWrapper, Is.Not.Null);
            Assert.That(stateWrapper, Is.TypeOf<LocalStateWrapper>());
        }

        [Test]
        public void verify_instance_of_ISessionStateSelector_is_DefaultSessionStateSelector()
        {
            var selector = _container.GetInstance<ISessionStateSelector>();
            Assert.That(selector, Is.Not.Null);
            Assert.That(selector, Is.TypeOf<DefaultSessionStateSelector>());
        }

        [Test]
        public void verify_instance_of_ISessionState_is_SessionStateWrapper()
        {
            var stateWrapper = _container.GetInstance<ISessionState>();
            Assert.That(stateWrapper, Is.Not.Null);
            Assert.That(stateWrapper, Is.TypeOf<SessionStateWrapper>());
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