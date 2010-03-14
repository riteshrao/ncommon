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
using Db4objects.Db4o;

namespace NCommon.Data.Db4o
{
    /// <summary>
    /// <see cref="IUnitOfWorkFactory"/> implementation for <see cref="Db4oUnitOfWork"/> instances.
    /// </summary>
    public class Db4oUnitOfWorkFactory : IUnitOfWorkFactory
    {
        static Func<IObjectContainer> _containerProvider;

        public static void SetContainerProvider(Func<IObjectContainer> containerProvider)
        {
            _containerProvider = containerProvider;       
        }

        /// <summary>
        /// Creates an instance of <see cref="IUnitOfWork"/>.
        /// </summary>
        /// <returns>An <see cref="IUnitOfWork"/> instance.</returns>
        public IUnitOfWork Create()
        {
            Guard.Against<InvalidOperationException>(_containerProvider == null,
                                                    "A IObjectContainer provider has not been specified. Please specify a " +
                                                     "provider using SetContainerProvider before creating Db4oUnitOfWork instances");
            return new Db4oUnitOfWork(_containerProvider());
        }
    }
}