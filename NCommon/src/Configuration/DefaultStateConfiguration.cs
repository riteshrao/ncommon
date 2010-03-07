using System;
using NCommon.State;
using NCommon.State.Impl;

namespace NCommon.Configuration.Impl
{
    public class DefaultStateConfiguration : IStateConfiguration
    {
        Type _customCacheType = null;
        Type _customSessionType = null;
        Type _customLocalStateType = null;
        Type _customApplicationStateType = null;

        public IStateConfiguration UseCustomCacheOf<T>() where T : ICacheState
        {
            _customCacheType = typeof (T);
            return this;
        }

        public IStateConfiguration UseCustomSessionStateOf<T>() where T : ISessionState
        {
            _customSessionType = typeof (T);
            return this;
        }

        public IStateConfiguration UseCustomLocalStateOf<T>() where T : ILocalState
        {
            _customLocalStateType = typeof (T);
            return this;
        }

        public IStateConfiguration UseCustomApplicationStateOf<T>() where T : IApplicationState
        {
            _customApplicationStateType = typeof (T);
            return this;
        }

        public void Configure(IContainerAdapter containerAdapter)
        {
            if (_customSessionType != null)
                containerAdapter.Register(typeof(ISessionState), _customSessionType);
            else
            {
                containerAdapter.Register<ISessionStateSelector, DefaultSessionStateSelector>();
                containerAdapter.Register<ISessionState, SessionStateWrapper>();
            }

            if (_customLocalStateType != null)
                containerAdapter.Register(typeof(ILocalState), _customLocalStateType);
            else
            {
                containerAdapter.Register<ILocalStateSelector, DefaultLocalStateSelector>();
                containerAdapter.Register<ILocalState, LocalStateWrapper>();
            }
            if (_customCacheType != null)
                containerAdapter.Register(typeof(ICacheState), _customCacheType);
            else
                containerAdapter.Register<ICacheState, HttpRuntimeCache>();
            if (_customApplicationStateType != null)
                containerAdapter.RegisterSingleton(typeof(IApplicationState), _customApplicationStateType);
            else
                containerAdapter.RegisterSingleton<IApplicationState, ApplicationState>();
        }
    }
}