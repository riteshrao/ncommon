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

namespace NCommon.Configuration
{
    /// <summary>
    /// Configuration interface exposed by NCommon to configure different services exposed by NCommon.
    /// </summary>
    public interface INCommonConfig
    {
        /// <summary>
        /// Configure NCommon state storage using a <see cref="IStateConfiguration"/> instance.
        /// </summary>
        /// <typeparam name="T">A <see cref="IStateConfiguration"/> type that can be used to configure
        /// state storage services exposed by NCommon.
        /// </typeparam>
        /// <returns><see cref="INCommonConfig"/></returns>
        INCommonConfig ConfigureState<T>() where T : IStateConfiguration, new();

        /// <summary>
        /// Configure NCommon state storage using a <see cref="IStateConfiguration"/> instance.
        /// </summary>
        /// <typeparam name="T">A <see cref="IStateConfiguration"/> type that can be used to configure
        /// state storage services exposed by NCommon.
        /// </typeparam>
        /// <param name="actions">An <see cref="Action{T}"/> delegate that can be used to perform
        /// custom actions on the <see cref="IStateConfiguration"/> instance.</param>
        /// <returns><see cref="INCommonConfig"/></returns>
        INCommonConfig ConfigureState<T>(Action<T> actions) where T : IStateConfiguration, new();

        /// <summary>
        /// Configure data providers used by NCommon.
        /// </summary>
        /// <typeparam name="T">A <see cref="IDataConfiguration"/> type that can be used to configure
        /// data providers for NCommon.</typeparam>
        /// <returns><see cref="INCommonConfig"/></returns>
        INCommonConfig ConfigureData<T>() where T : IDataConfiguration, new();

        /// <summary>
        /// Configure data providers used by NCommon.
        /// </summary>
        /// <typeparam name="T">A <see cref="IDataConfiguration"/> type that can be used to configure
        /// data providers for NCommon.</typeparam>
        /// <param name="actions">An <see cref="Action{T}"/> delegate that can be used to perform
        /// custom actions on the <see cref="IDataConfiguration"/> instance.</param>
        /// <returns><see cref="INCommonConfig"/></returns>
        INCommonConfig ConfigureData<T>(Action<T> actions) where T : IDataConfiguration, new();
    }
}