﻿#region license
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

namespace NCommon.Validation
{
    /// <summary>
    /// Provides a contract that defines a validation rule that provides validation logic  for an entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity this validation rule is applicable for.</typeparam>
    public interface IValidationRule<TEntity>
    {
        /// <summary>
        /// Gets the message of the validation rule.
        /// </summary>
        string ValidationMessage { get; }

        /// <summary>
        /// Gets a generic or specific name of a property that was validated.
        /// </summary>
        string ValidationProperty { get; }

        /// <summary>
        /// Validates whether the entity violates the validation rule or not.
        /// </summary>
        /// <param name="entity">The <typeparamref name="TEntity"/> entity instance to validate.</param>
        /// <returns>Should return true if the entity instance is valid, else false.</returns>
        bool Validate(TEntity entity);
    }
}
