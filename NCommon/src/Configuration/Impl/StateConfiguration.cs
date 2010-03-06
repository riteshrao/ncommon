using NCommon.State;

namespace NCommon.Configuration.Impl
{
    public class StateConfiguration : IStateConfiguration
    {
        readonly IContainer _container;

        public StateConfiguration(IContainer container)
        {
            _container = container;
        }

        public IStateConfiguration Cache<T>() where T : ICacheState
        {
            _container.ReplaceComponent<ICacheState, T>();
            return this;
        }

        public IStateConfiguration Session<T>() where T : ISessionState
        {
            _container.ReplaceComponent<ISessionState, T>();
            return this;
        }

        public IStateConfiguration LocalState<T>() where T : ILocalState
        {
            _container.ReplaceComponent<ILocalState, T>();
            return this;
        }
    }
}