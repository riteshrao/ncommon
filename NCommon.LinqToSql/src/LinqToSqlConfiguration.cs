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
using System.Data.Linq;
using NCommon.Configuration;
using NCommon.DataServices.Transactions;
using NCommon.DependencyInjection;
using NCommon.ObjectAccess;

namespace NCommon.LinqToSql
{
    /// <summary>
    /// Implementatio of <see cref="IDataConfiguration"/> that configured NCommon to use Linq To Sql
    /// </summary>
    public class LinqToSqlConfiguration : IDataConfiguration
    {
        readonly LinqToSqlUnitOfWorkFactory _factory = new LinqToSqlUnitOfWorkFactory();

        /// <summary>
        /// Registers a <see cref="DataContext"/> provider.
        /// </summary>
        /// <param name="contextProvider">A <see cref="Func{T}"/> of type <see cref="DataContext"/>.</param>
        /// <returns><see cref="LinqToSqlConfiguration"/></returns>
        public LinqToSqlConfiguration WithDataContext(Func<DataContext> contextProvider)
        {
            _factory.RegisterDataContextProvider(contextProvider);
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
            containerAdapter.RegisterGeneric(typeof(IRepository<>), typeof(LinqToSqlRepository<>));
        }
    }
}