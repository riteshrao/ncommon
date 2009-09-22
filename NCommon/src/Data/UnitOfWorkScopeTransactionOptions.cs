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
    /// <summary>
    /// Specifies the transaction options for creating a <see cref="UnitOfWorkScopeTransaction"/> instance.
    /// </summary>
    [Flags]
    public enum UnitOfWorkScopeTransactionOptions
    {
        /// <summary>
        /// Specifies that if a <see cref="UnitOfWorkScopeTransaction"/> instance with a compatible isolation level
        /// exists for the current thread / request, use that transaction or create a new one. <see cref="UnitOfWorkScope"/>
        /// defaults to this option.
        /// </summary>
        UseCompatible = 1,
        /// <summary>
        /// Specifies that the a new <see cref="UnitOfWorkScopeTransaction"/> should be created irrespective of whether
        /// a compaible transaction already exists.
        /// </summary>
        CreateNew = 2,
		/// <summary>
		/// Specifies that the <see cref="UnitOfWorkScopeTransaction"/> should be marked as completed and
		/// committed automatically when the current <see cref="UnitOfWorkScope"/> is disposed.
		/// </summary>
		/// <remarks>
		/// Attribution: AutoComplete implementation based on patch provided by Pablo Ruiz [http://humm4life.blogspot.com/2009/04/adding-transaction-autocomplete-support.html].
		/// </remarks>
		AutoComplete = 4
    }
}