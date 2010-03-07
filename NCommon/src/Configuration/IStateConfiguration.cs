using NCommon.State;

namespace NCommon.Configuration
{
    public interface IStateConfiguration
    {
        IStateConfiguration UseCustomCacheOf<T>() where T : ICacheState;
        IStateConfiguration UseCustomSessionStateOf<T> () where T : ISessionState;
        IStateConfiguration UseCustomLocalStateOf<T>() where T : ILocalState;
        IStateConfiguration UseCustomApplicationStateOf<T>() where T : IApplicationState;
        void Configure(IContainerAdapter containerAdapter);
    }
}