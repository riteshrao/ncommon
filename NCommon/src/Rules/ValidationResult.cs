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
using System.Collections.Generic;

namespace NCommon.Rules
{
    /// <summary>
    /// Contains the result of a <see cref="IEntityValidator{TEntity}.Validate"/> method call.
    /// </summary>
    [Serializable]
    public class ValidationResult
    {
        private readonly List<ValidationError> _errors = new List<ValidationError>();

        /// <summary>
        /// Gets wheater the validation operation on an entity was valid or not.
        /// </summary>
        public bool IsValid { get { return _errors.Count == 0; } }

        /// <summary>
        /// Gets an <see cref="IEnumerable{ValidationError}"/> that can be used to enumerate over
        /// the validation errors as a result of a <see cref="IEntityValidator{TEntity}.Validate"/> method
        /// call.
        /// </summary>
        public IEnumerable<ValidationError> Errors
        {
            get
            {
                foreach (var error in _errors)
                    yield return error;
            }
        }

        /// <summary>
        /// Adds a validation error into the result.
        /// </summary>
        /// <param name="error"></param>
        public void AddError(ValidationError error)
        {
            _errors.Add(error);    
        }

        /// <summary>
        /// Removes a validation error from the result.
        /// </summary>
        /// <param name="error"></param>
        public void RemoveError(ValidationError error)
        {
            if (_errors.Contains(error))
                _errors.Remove(error);
        }
    }
}
