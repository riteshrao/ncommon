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

namespace NCommon.DataServices.Transactions
{
    /// <summary>
    /// Implemented by a transaction manager that manages unit of work transactions.
    /// </summary>
    public interface IUnitOfWorkTransactionManager : IDisposable
    {
        /// <summary>
        /// Returns the current <see cref="IUnitOfWork"/>.
        /// </summary>
        IUnitOfWork CurrentUnitOfWork { get;}

        /// <summary>
        /// Enlists a <see cref="UnitOfWorkScope"/> instance with the transaction manager,
        /// with the specified transaction mode.
        /// </summary>
        /// <param name="scope">The <see cref="IUnitOfWorkScope"/> to register.</param>
        /// <param name="mode">A <see cref="TransactionMode"/> enum specifying the transaciton
        /// mode of the unit of work.</param>
        void EnlistScope(IUnitOfWorkScope scope, TransactionMode mode);
    }
}