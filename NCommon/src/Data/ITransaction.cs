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

namespace NCommon.Data
{
    ///<summary>
    /// Encapsulates a data store transaction. Used by <see cref="IUnitOfWork"/> to flush changes
    /// under a transaction.
    ///</summary>
    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// Event raised when the transaction has been comitted.
        /// </summary>
        event EventHandler TransactionCommitted;

        /// <summary>
        /// Event raised when the transaction has been rolledback.
        /// </summary>
        event EventHandler TransactionRolledback;

        /// <summary>
        /// Commits the changes made to the data store.
        /// </summary>
        /// <remarks>Implementors MUST raise the <see cref="TransactionCommitted"/> event.</remarks>
        void Commit();

        /// <summary>
        /// Rollsback any changes made.
        /// </summary>
        /// <remarks>Implementors MUST raise the <see cref="TransactionRolledback"/> event.</remarks>
        void Rollback();
    }
}