using NCommon.State;

namespace NCommon.Configuration.Impl
{
    public class StateConfiguration : IStateConfiguration
    {
        readonly IContainerAdapter _containerAdapter;

        public StateConfiguration(IContainerAdapter containerAdapter)
        {
            _containerAdapter = containerAdapter;
        }

        public IStateConfiguration Cache<T>() where T : ICacheState
        {
            _containerAdapter.ReplaceComponent<ICacheState, T>();
            return this;
        }

        public IStateConfiguration Session<T>() where T : ISessionState
        {
            _containerAdapter.ReplaceComponent<ISessionState, T>();
            return this;
        }

        public IStateConfiguration LocalState<T>() where T : ILocalState
        {
            _containerAdapter.ReplaceComponent<ILocalState, T>();
            return this;
        }
    }
}