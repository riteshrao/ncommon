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
using NHibernate;

namespace NCommon.Data.NHibernate
{
    /// <summary>
    /// Implements the <see cref="IUnitOfWorkFactory"/> interface to provide an implementation of a factory
    /// that creates <see cref="NHUnitOfWork"/> instances.
    /// </summary>
    public class NHUnitOfWorkFactory : IUnitOfWorkFactory
    {
        NHSessionResolver _sessionResolver = new NHSessionResolver();

        /// <summary>
        /// Registers a <see cref="Func{T}"/> of type <see cref="ISessionFactory"/> provider with the unit of work factory.
        /// </summary>
        /// <param name="factoryProvider">A <see cref="Func{T}"/> of type <see cref="ISessionFactory"/> instance.</param>
        public void RegisterSessionFactoryProvider(Func<ISessionFactory> factoryProvider)
        {
            Guard.Against<ArgumentNullException>(factoryProvider == null,
                                                 "Invalid session factory provider registration. " +
                                                 "Expected a non-null Func<ISessionFactory> instance.");
            _sessionResolver.RegisterSessionFactoryProvider(factoryProvider);
        }
        
        /// <summary>
        /// Creates a new instance of <see cref="IUnitOfWork"/>.
        /// </summary>
        /// <returns></returns>
        public IUnitOfWork Create()
        {
            Guard.Against<InvalidOperationException>(
                _sessionResolver.SessionFactoriesRegistered == 0,
                "No session factory providers have been registered. You must register ISessionFactory providers using " +
                "the RegisterSessionFactoryProvider method or use NCommon.Configure class to configure NCommon.NHibernate " +
                "using the NHConfiguration class and register ISessionFactory instances using the WithSessionFactory method.");
            return new NHUnitOfWork(_sessionResolver);
        }
    }
}