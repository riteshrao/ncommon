#region license
//Copyright 2010 Ritesh Rao 

//Licensed under the Apache License, Version 2.0 (the "License"); 
//you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 

//http://www.apache.org/licenses/LICENSE-2.0 

//Unless required by applicable law or agreed to in writing, software 
//distributed under the License is distributed on an "AS IS" BASIS, 
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and 
//limitations under the License. 
#endregion

using System;
using NCommon.State;
using NCommon.State.Impl;

namespace NCommon.Configuration
{
    /// <summary>
    /// Default implementation of <see cref="IStateConfiguration"/> that allows configuring
    /// state storage in NCommon.
    /// </summary>
    public class DefaultStateConfiguration : IStateConfiguration
    {
        Type _customCacheType;
        Type _customSessionType;
        Type _customLocalStateType;
        Type _customApplicationStateType;

        /// <summary>
        /// Instructs NCommon to use the custom <see cref="ICacheState"/> type as the cache storage.
        /// </summary>
        /// <typeparam name="T">A type that implements the <see cref="ICacheState"/> interface.</typeparam>
        /// <returns>The <see cref="DefaultStateConfiguration"/> instance.</returns>
        public DefaultStateConfiguration UseCustomCacheOf<T>() where T : ICacheState
        {
            _customCacheType = typeof (T);
            return this;
        }

        /// <summary>
        /// Instructs NCommon to use a custom <see cref="ISessionState"/> type as the session state storage.
        /// </summary>
        /// <typeparam name="T">A type that implements the <see cref="ISessionState"/> interface.</typeparam>
        /// <returns>The <see cref="DefaultStateConfiguration"/> instance</returns>
        public DefaultStateConfiguration UseCustomSessionStateOf<T>() where T : ISessionState
        {
            _customSessionType = typeof (T);
            return this;
        }

        /// <summary>
        /// Instructs NCommon to use a custom <see cref="ILocalState"/> type as the local state storage.
        /// </summary>
        /// <typeparam name="T">A type that implements the <see cref="ILocalState"/> interface.</typeparam>
        /// <returns>The <see cref="DefaultStateConfiguration"/> instance.</returns>
        public DefaultStateConfiguration UseCustomLocalStateOf<T>() where T : ILocalState
        {
            _customLocalStateType = typeof (T);
            return this;
        }

        /// <summary>
        /// Instructs NCommon to use a custom <see cref="IApplicationState"/> type as the application stage storage.
        /// </summary>
        /// <typeparam name="T">A type that implements the <see cref="IApplicationState"/> interface.</typeparam>
        /// <returns>The <see cref="DefaultStateConfiguration"/> instance.</returns>
        public DefaultStateConfiguration UseCustomApplicationStateOf<T>() where T : IApplicationState
        {
            _customApplicationStateType = typeof (T);
            return this;
        }

        /// <summary>
        /// Called by NCommon <see cref="Configure"/> to configure state storage.
        /// </summary>
        /// <param name="containerAdapter">The <see cref="IContainerAdapter"/> instance that can be
        /// used to register state storage components.</param>
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

            containerAdapter.Register<IState, State.Impl.State>();
        }
    }
}