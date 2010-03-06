using NCommon.State;

namespace NCommon.Configuration
{
    public interface IStateConfiguration
    {
        IStateConfiguration Cache<T>() where T : ICacheState;
        IStateConfiguration Session<T> () where T : ISessionState;
        IStateConfiguration LocalState<T>() where T : ILocalState;
    }
}