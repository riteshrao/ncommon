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
using System.Data;
using System.Data.Objects;

namespace NCommon.Data.EntityFramework
{
    /// <summary>
    /// Implements the <see cref="IUnitOfWorkFactory"/> interface to provide an implementation of a factory
    /// that creates <see cref="EFUnitOfWork"/> instances.
    /// </summary>
    public class EFUnitOfWorkFactory : IUnitOfWorkFactory
    {
        readonly EFUnitOfWorkSettings _settings = new EFUnitOfWorkSettings
        {
            DefaultIsolation = IsolationLevel.ReadCommitted,
            SessionResolver = new EFSessionResolver()
        };
        
        /// <summary>
        /// Gets or sets the default isolation level of <see cref="EFUnitOfWork"/> instances that the 
        /// factory creates.
        /// </summary>
        /// <value>The default <see cref="IsolationLevel"/> of <see cref="EFUnitOfWork"/> instances.</value>
        public IsolationLevel DefaultIsolation
        {
            get { return _settings.DefaultIsolation; }
            set { _settings.DefaultIsolation = value; }
        }

        /// <summary>
        /// Registers a <see cref="Func{T}"/> of type <see cref="ObjectContext"/> provider that can be used
        /// to resolve instances of <see cref="ObjectContext"/>.
        /// </summary>
        /// <param name="contextProvider">A <see cref="Func{T}"/> of type <see cref="ObjectContext"/>.</param>
        public void RegisterObjectContextProvider(Func<ObjectContext> contextProvider)
        {
            Guard.Against<ArgumentNullException>(contextProvider == null,
                                                 "Invalid object context provider registration. " +
                                                 "Expected a non-null Func<ObjectContext> instance.");
            _settings.SessionResolver.RegisterObjectContextProvider(contextProvider);
        }

        /// <summary>
        /// Creates a new instance of <see cref="IUnitOfWork"/>.
        /// </summary>
        /// <returns>Instances of <see cref="EFUnitOfWork"/>.</returns>
        public IUnitOfWork Create()
        {
            Guard.Against<InvalidOperationException>(
               _settings.SessionResolver.ObjectContextsRegistered == 0,
               "No ObjectContext providers have been registered. You must register ObjectContext providers using " +
               "the RegisterObjectContextProvider method or use NCommon.Configure class to configure NCommon.EntityFramework " +
               "using the EFConfiguration class and register ObjectContext instances using the WithObjectContext method.");
            
            return new EFUnitOfWork(_settings);
        }
    }
}
