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
using System.Transactions;
using Common.Logging;

namespace NCommon.DataServices.Transactions
{
    /// <summary>
    /// Helper class to create <see cref="TransactionScope"/> instances.
    /// </summary>
    public static class TransactionScopeHelper
    {
        static readonly ILog Logger = LogManager.GetLogger(typeof (TransactionScopeHelper));

        ///<summary>
        ///</summary>
        ///<param name="isolationLevel"></param>
        ///<param name="txMode"></param>
        ///<returns></returns>
        ///<exception cref="NotImplementedException"></exception>
        public static TransactionScope CreateScope(IsolationLevel isolationLevel, TransactionMode txMode)
        {
            if (txMode == TransactionMode.New)
            {
                Logger.Debug(x => x("Creating a new TransactionScope with TransactionScopeOption.RequiresNew"));
                return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = isolationLevel });
            }
            if (txMode == TransactionMode.Supress)
            {
                Logger.Debug(x => x("Creating a new TransactionScope with TransactionScopeOption.Supress"));
                return new TransactionScope(TransactionScopeOption.Suppress);
            }
            Logger.Debug(x => x("Creating a new TransactionScope with TransactionScopeOption.Required"));
            return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = isolationLevel });
        }
    }
}