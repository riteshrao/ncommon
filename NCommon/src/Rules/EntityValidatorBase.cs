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
using NCommon.Extensions;

namespace NCommon.Rules
{
    ///<summary>
    /// Base class that implementors of <see cref="IEntityValidator{TEntity}"/> can use to 
    /// provide validation logic for their entities.
    ///</summary>
    ///<typeparam name="TEntity"></typeparam>
    public abstract class EntityValidatorBase<TEntity> : IEntityValidator<TEntity> where TEntity : class
    {
        //The internal dictionary used to store rule sets.
        private readonly Dictionary<string, IValidationRule<TEntity>> _validations = new Dictionary<string, IValidationRule<TEntity>>();

        /// <summary>
        /// Adds a <see cref="IValidationRule{TEntity}"/> instance to the entity validator.
        /// </summary>
        /// <param name="rule">The <see cref="IValidationRule{TEntity}"/> instance to add.</param>
        /// <param name="ruleName">string. The unique name assigned to the validation rule.</param>
        protected void AddValidation(string ruleName, IValidationRule<TEntity> rule)
        {
            Guard.Against<ArgumentNullException>(rule == null,
                                                 "Cannot add a null rule instance. Expected a non null reference.");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(ruleName),
                                                 "Cannot add a rule with an empty or null rule name.");
            Guard.Against<ArgumentException>(_validations.ContainsKey(ruleName),
                                             "Another rule with the same name already exists. Cannot add duplicate rules.");

            _validations.Add(ruleName, rule);
        }

        /// <summary>
        /// Removes a previously added rule, specified with the <paramref name="ruleName"/>, from the evaluator.
        /// </summary>
        /// <param name="ruleName">string. The name of the rule to remove.</param>
        protected void RemoveValidation(string ruleName)
        {
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(ruleName), "Expected a non empty and non-null rule name.");
            _validations.Remove(ruleName);
        }

        /// <summary>
        /// Validates an entity against all validations defined for the entity.
        /// </summary>
        /// <param name="entity">The <typeparamref name="TEntity"/> to validate.</param>
        /// <returns>A <see cref="ValidationResult"/> that contains the results of the validation.</returns>
        public ValidationResult Validate(TEntity entity)
        {
            ValidationResult result = new ValidationResult();
            _validations.Keys.ForEach(x =>
                                          {
                                              IValidationRule<TEntity> rule = _validations[x];
                                              if (!rule.Validate(entity))
                                                  result.AddError(new ValidationError(rule.ValidationMessage,
                                                                                        rule.ValidationProperty));
                                          });
            return result;
        }
    }

}
