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

using System.Transactions;
using Common.Logging;

namespace NCommon.Data.Impl
{
    /// <summary>
    /// Helper class to create <see cref="TransactionScope"/> instances.
    /// </summary>
    public static class TransactionScopeHelper
    {
        static readonly ILog Logger = LogManager.GetLogger(typeof (TransactionScopeHelper));

        /// <summary>
        /// Creates a <see cref="TransactionScope"/> with the specified isolation level.
        /// </summary>
        /// <param name="isolationLevel">The <see cref="IsolationLevel"/> of the scope.</param>
        /// <returns>A <see cref="TransactionScope"/> instance.</returns>
        /// <remarks>If an ambient transaction with the same isolation level exists, this method
        /// will create a new instance of <see cref="TransactionScope"/> that is part of the ambient
        /// transaction, else it will create a new scope with the specified isolation level.</remarks>
        public static TransactionScope CreateScope(IsolationLevel isolationLevel)
        {
            if (Transaction.Current == null)
                return CreateNewScope(isolationLevel);

            Logger.Debug(x => x("Creating a TransactionScope enlisted in an existing parent ambient transaction."));
            return new TransactionScope(Transaction.Current);
        }

        /// <summary>
        /// Creates a <see cref="TransactionScope"/> with the specified isolation level and
        /// does not enlist as part of an existing ambient transaction.
        /// </summary>
        /// <param name="isolationLevel">The <see cref="IsolationLevel"/> of the scope.</param>
        /// <returns>An instance of <see cref="TransactionScope"/>.</returns>
        public static TransactionScope CreateNewScope(IsolationLevel isolationLevel)
        {
            Logger.Debug(x => x("Creating a new un-enlisted TransactionScope with IsolationLevel {0}", isolationLevel));
            return new TransactionScope(TransactionScopeOption.RequiresNew,
                                        new TransactionOptions {IsolationLevel = isolationLevel});
        }
    }
}