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
using System.Data.Linq;

namespace NCommon.Data.LinqToSql
{
    /// <summary>
    /// Implements the <see cref="IUnitOfWorkFactory"/> interface to provide an implementation of a factory
    /// that creates <see cref="LinqToSqlUnitOfWork"/> instances.
    /// </summary>
    public class LinqToSqlUnitOfWorkFactory : IUnitOfWorkFactory
    {
        LinqToSqlSessionResolver _resolver = new LinqToSqlSessionResolver();

        /// <summary>
        /// Registers a <see cref="Func{T}"/> of type <see cref="DataContext"/> provider that can be used to 
        /// get instances of <see cref="DataContext"/>.
        /// </summary>
        /// <param name="contextProvider">An instance of <see cref="Func{T}"/> of type <see cref="DataContext"/>.</param>
        public void RegisterDataContextProvider(Func<DataContext> contextProvider)
        {
            Guard.Against<ArgumentNullException>(contextProvider == null,
                                                 "Cannot register a null Func<DataContext> provider with the factory.");
            _resolver.RegisterDataContextProvider(contextProvider);
        }

        /// <summary>
        /// Creates a new instance of <see cref="IUnitOfWork"/>.
        /// </summary>
        /// <returns>Instances of <see cref="LinqToSqlUnitOfWork"/>.</returns>
        public IUnitOfWork Create()
        {
            Guard.Against<InvalidOperationException>(
                 _resolver.DataContextsRegistered == 0,
                 "No DataContext providers have been registered. You must register DataContext providers using " +
                 "the RegisterDataContextProvider method or use NCommon.Configure class to configure NCommon.LinqToSql " +
                 "using the LinqToSqlConfiguration class and register DataContext instances using the WithDataContext method.");
            return new LinqToSqlUnitOfWork(_resolver);
        }
    }
}
