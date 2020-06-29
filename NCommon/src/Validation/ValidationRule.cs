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


namespace NCommon.Validation
{
    /// <summary>
    /// Implements the <see cref="IValidationRule{TEntity}"/> interface and inherits from the
    /// <see cref="SpecificationRuleBase{TEntity}"/> to provide a very basic implementation of an
    /// entity validation rule that uses specifications as underlying rule logic.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ValidationRule<TEntity> : SpecificationRuleBase<TEntity>, IValidationRule<TEntity>
    {
        private readonly string _message;
        private readonly string _property;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="ValidationRule{TEntity}"/> class.
        /// </summary>
        /// <param name="message">string. The validation message associated with the rule.</param>
        /// <param name="property">string. The generic or specific name of the property that was validated.</param>
        /// <param name="rule"></param>
        public ValidationRule(ISpecification<TEntity> rule, string message, string property) : base(rule)
        {
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(message), "Please provide a valid non null value for the validationMessage parameter.");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(property), "Please provide a valid non null value for the validationProperty parameter.");
            _message = message;
            _property = property;
        }

        /// <summary>
        /// Gets the message of the validation rule.
        /// </summary>
        public string ValidationMessage
        {
            get { return _message; }
        }

        /// <summary>
        /// Gets a generic or specific name of a property that was validated.
        /// </summary>
        public string ValidationProperty
        {
            get { return _property; }
        }

        /// <summary>
        /// Validates whether the entity violates the validation rule or not.
        /// </summary>
        /// <param name="entity">The <typeparamref name="TEntity"/> entity instance to validate.</param>
        /// <returns>Should return true if the entity instance is valid, else false.</returns>
        public bool Validate(TEntity entity)
        {
            return IsSatisfied(entity);
        }
    }
}
