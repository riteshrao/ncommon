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
using System.Data.Objects;

namespace NCommon.Data.EntityFramework
{
    /// <summary>
    /// Implements the <see cref="IUnitOfWorkFactory"/> interface to provide an implementation of a factory
    /// that creates <see cref="EFUnitOfWork"/> instances.
    /// </summary>
    public class EFUnitOfWorkFactory : IUnitOfWorkFactory
    {
        #region fields
        private static Func<ObjectContext> _objectContextProvider;
        private static readonly object _objectContextProviderLock = new object();
        #endregion

        #region methods
        /// <summary>
        /// Sets delegate that needs to be used for getting <see cref="ObjectContext"/> instances.
        /// </summary>
        /// <param name="provider"><see cref="Func{TResult}"/> The delegate when called creates insances of 
        /// <see cref="ObjectContext"/> instances.</param>
        public static void SetObjectContextProvider(Func<ObjectContext> provider)
        {
            lock (_objectContextProviderLock)
                _objectContextProvider = provider;
        }
        #endregion

        #region Implementation of IUnitOfWorkFactory
        /// <summary>
        /// Creates a new instance of <see cref="IUnitOfWork"/>.
        /// </summary>
        /// <returns>Instances of <see cref="EFUnitOfWork"/>.</returns>
        public IUnitOfWork Create()
        {
            Guard.Against<InvalidOperationException>(_objectContextProvider == null,
                                                     "A ObjectContext provider has not been specified. Please specify set a " +
                                                     "provider using SetObjectContextProvider before creating EFUnitOfWork instances");
            ObjectContext context;
            lock (_objectContextProviderLock)
            {
                context = _objectContextProvider();
            }
            return new EFUnitOfWork(new EFSession(context));
        }
        #endregion
    }
}
