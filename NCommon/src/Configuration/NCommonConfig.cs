#region license
//Copyright 2008 Ritesh Rao 

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
using NCommon.Context;

namespace NCommon.Configuration
{
    ///<summary>
    /// Default implementation of <see cref="INCommonConfig"/> class.
    ///</summary>
    public class NCommonConfig : INCommonConfig
    {
        readonly IContainerAdapter _containerAdapter;

        ///<summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="NCommonConfig"/>  class.
        ///</summary>
        ///<param name="containerAdapter">An instance of <see cref="IContainerAdapter"/> that can be
        /// used to register components.</param>
        public NCommonConfig(IContainerAdapter containerAdapter)
        {
            _containerAdapter = containerAdapter;
            InitializeDefaults();
        }

        /// <summary>
        /// Registers default components for NCommon.
        /// </summary>
        void InitializeDefaults()
        {
            _containerAdapter.Register<IContext, Context.Impl.Context>();
        }

        /// <summary>
        /// Configure NCommon state storage using a <see cref="IStateConfiguration"/> instance.
        /// </summary>
        /// <typeparam name="T">A <see cref="IStateConfiguration"/> type that can be used to configure
        /// state storage services exposed by NCommon.
        /// </typeparam>
        /// <returns><see cref="INCommonConfig"/></returns>
        public INCommonConfig ConfigureState<T>() where T : IStateConfiguration, new()
        {
            var configuration = (T) Activator.CreateInstance(typeof (T));
            configuration.Configure(_containerAdapter);
            return this;
        }

        /// <summary>
        /// Configure NCommon state storage using a <see cref="IStateConfiguration"/> instance.
        /// </summary>
        /// <typeparam name="T">A <see cref="IStateConfiguration"/> type that can be used to configure
        /// state storage services exposed by NCommon.
        /// </typeparam>
        /// <param name="actions">An <see cref="Action{T}"/> delegate that can be used to perform
        /// custom actions on the <see cref="IStateConfiguration"/> instance.</param>
        /// <returns><see cref="INCommonConfig"/></returns>
        public INCommonConfig ConfigureState<T>(Action<T> actions) where T : IStateConfiguration, new()
        {
            var configuration = (T) Activator.CreateInstance(typeof (T));
            actions(configuration);
            configuration.Configure(_containerAdapter);
            return this;
        }

        /// <summary>
        /// Configure data providers used by NCommon.
        /// </summary>
        /// <typeparam name="T">A <see cref="IDataConfiguration"/> type that can be used to configure
        /// data providers for NCommon.</typeparam>
        /// <returns><see cref="INCommonConfig"/></returns>
        public INCommonConfig ConfigureData<T>() where T : IStateConfiguration, new()
        {
            var datConfiguration = (T) Activator.CreateInstance(typeof (T));
            datConfiguration.Configure(_containerAdapter);
            return this;
        }

        /// <summary>
        /// Configure data providers used by NCommon.
        /// </summary>
        /// <typeparam name="T">A <see cref="IDataConfiguration"/> type that can be used to configure
        /// data providers for NCommon.</typeparam>
        /// <param name="actions">An <see cref="Action{T}"/> delegate that can be used to perform
        /// custom actions on the <see cref="IDataConfiguration"/> instance.</param>
        /// <returns><see cref="INCommonConfig"/></returns>
        public INCommonConfig ConfigureData<T>(Action<T> actions) where T : IDataConfiguration, new()
        {
            var dataConfiguration = (T) Activator.CreateInstance(typeof (T));
            actions(dataConfiguration);
            dataConfiguration.Configure(_containerAdapter);
            return this;
        }
    }
}