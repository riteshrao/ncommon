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

namespace NCommon.Data
{
    ///<summary>
    ///</summary>
    public interface IUnitOfWorkScope : IDisposable
    {
        /// <summary>
        /// Event fired when the scope is comitting.
        /// </summary>
        event Action<IUnitOfWorkScope> ScopeComitting;

        /// <summary>
        /// Event fired when the scope is rollingback.
        /// </summary>
        event Action<IUnitOfWorkScope> ScopeRollingback;

        /// <summary>
        /// Gets the unique Id of the <see cref="UnitOfWorkScope"/>.
        /// </summary>
        /// <value>A <see cref="Guid"/> representing the unique Id of the scope.</value>
        Guid ScopeId { get; }

        ///<summary>
        /// Commits the current running transaction in the scope.
        ///</summary>
        void Commit();

        /// <summary>
        /// Marks the scope as completed.
        /// Used for internally by NCommon and should not be used by consumers.
        /// </summary>
        void Complete();
    }
}