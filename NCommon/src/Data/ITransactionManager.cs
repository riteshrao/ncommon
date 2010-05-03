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

namespace NCommon.Data
{
    /// <summary>
    /// Implemented by a transaction manager that manages unit of work transactions.
    /// </summary>
    public interface ITransactionManager
    {
        /// <summary>
        /// Returns the current <see cref="IUnitOfWork"/>.
        /// </summary>
        IUnitOfWork CurrentUnitOfWork { get;}
        /// <summary>
        /// Enlists a <see cref="UnitOfWorkScope"/> instance with the transaction manager.
        /// </summary>
        /// <param name="scope">bool. True if the scope should be enlisted in a new transaction, else
        /// false if the scope should participate in the existing transaction</param>
        /// <param name="newTransaction"></param>
        void EnlistScope(IUnitOfWorkScope scope, bool newTransaction);
    }
}