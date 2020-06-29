﻿#region license
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

using CommonServiceLocator;
using NCommon.DataServices.Transactions;
using NCommon.StateStorage;
using Rhino.Mocks;


namespace NCommon.Testing
{
    /// <summary>
    /// Configures NCommon for unit tests.
    /// </summary>
    public static class Configure
    {
        ///<summary>
        /// Configures NCommon using the specified mocked <see cref="IServiceLocator"/> instance.
        ///</summary>
        ///<param name="mockLocator">The <see cref="IServiceLocator"/> instance.</param>
        public static void Using(IServiceLocator mockLocator)
        {
            mockLocator.Stub(x => x.GetInstance<IState>()).Return(new FakeState());
            UnitOfWorkManager.SetTransactionManagerProvider(() => MockRepository.GenerateStub<IUnitOfWorkTransactionManager>());
        }
    }
}
