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
using System.Data.Linq;

namespace NCommon.Data.LinqToSql
{
    /// <summary>
    /// Implements the <see cref="IUnitOfWorkFactory"/> interface to provide an implementation of a factory
    /// that creates <see cref="LinqToSqlUnitOfWork"/> instances.
    /// </summary>
    public class LinqToSqlUnitOfWorkFactory : IUnitOfWorkFactory
    {
        #region fields
        private static Func<DataContext> _dataContextProvider;
        private static readonly object _dataContextProviderLock = new object();
        #endregion

        #region methods
        /// <summary>
        /// Sets delegate that needs to be used for getting <see cref="DataContext"/> instances.
        /// </summary>
        /// <param name="provider"><see cref="Func{TResult}"/> The delegate when called creates insances of 
        /// <see cref="DataContext"/> instances.</param>
        public static void SetDataContextProvider (Func<DataContext> provider)
        {
            lock (_dataContextProviderLock)
            {
                _dataContextProvider = provider;
            }
        }
        #endregion

        #region Implementation of IUnitOfWorkFactory

        /// <summary>
        /// Creates a new instance of <see cref="IUnitOfWork"/>.
        /// </summary>
        /// <returns>Instances of <see cref="LinqToSqlUnitOfWork"/>.</returns>
        public IUnitOfWork Create()
        {
            Guard.Against<InvalidOperationException>(_dataContextProvider == null,
                                                     "A DataContext provider has not been specified. Please specify set a " + 
                                                     "provider using SetDataContextProvider before creating LinqToUnitOfWork instances");
            DataContext context;
            lock (_dataContextProviderLock)
                context = _dataContextProvider();

            return new LinqToSqlUnitOfWork(new LinqUnitOfWorkDataContext(context));
        }
        #endregion
    }
}
