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
using NCommon.Configuration;

namespace NCommon.Data.EntityFramework
{
    /// <summary>
    /// Implementation of <see cref="IDataConfiguration"/> for Entity Framework.
    /// </summary>
    public class EFConfiguration : IDataConfiguration
    {
        readonly EFUnitOfWorkFactory _factory = new EFUnitOfWorkFactory();

        /// <summary>
        /// Specifies the default <see cref="IsolationLevel"/> of unit of work instances.
        /// </summary>
        /// <param name="isolationLevel"><see cref="IsolationLevel"/>. The default isolation level.</param>
        /// <returns><see cref="EFConfiguration"/>.</returns>
        public EFConfiguration WithDefaultIsolation(IsolationLevel isolationLevel)
        {
            _factory.DefaultIsolation = isolationLevel;
            return this;
        }

        /// <summary>
        /// Configures unit of work instances to use the specified <see cref="ObjectContext"/>.
        /// </summary>
        /// <param name="objectContextProvider">A <see cref="Func{T}"/> of type <see cref="ObjectContext"/>
        /// that can be used to construct <see cref="ObjectContext"/> instances.</param>
        /// <returns><see cref="EFConfiguration"/></returns>
        public EFConfiguration WithObjectContext(Func<ObjectContext> objectContextProvider)
        {
            Guard.Against<ArgumentNullException>(objectContextProvider == null,
                                                 "Expected a non-null Func<ObjectContext> instance.");
            _factory.RegisterObjectContextProvider(objectContextProvider);
            return this;
        }

        /// <summary>
        /// Called by NCommon <see cref="Configure"/> to configure data providers.
        /// </summary>
        /// <param name="containerAdapter">The <see cref="IContainerAdapter"/> instance that allows
        /// registering components.</param>
        public void Configure(IContainerAdapter containerAdapter)
        {
            containerAdapter.RegisterInstance<IUnitOfWorkFactory>(_factory);
            containerAdapter.Register(typeof(IRepository<>), typeof(EFRepository<>));
        }
    }
}