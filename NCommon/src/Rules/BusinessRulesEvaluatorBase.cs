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
using System.Collections.Generic;
using NCommon.Extensions;

namespace NCommon.Rules
{
    ///<summary>
    /// A base class that implementors of <see cref="IBusinessRulesEvaluator{TEntity}"/> can use to provide
    /// business rule evaulation logic for their entites.
    ///</summary>
    ///<typeparam name="TEntity"></typeparam>
    public abstract class BusinessRulesEvaluatorBase<TEntity> : IBusinessRulesEvaluator<TEntity> where TEntity : class
    {
        //The internal dictionary used to store rule sets.
        private readonly Dictionary<string, IBusinessRule<TEntity>> _ruleSets = new Dictionary<string, IBusinessRule<TEntity>>();

        /// <summary>
        /// Adds a <see cref="IBusinessRule{TEntity}"/> instance to the rules evaluator.
        /// </summary>
        /// <param name="rule">The <see cref="IBusinessRule{TEntity}"/> instance to add.</param>
        /// <param name="ruleName">string. The unique name assigned to the business rule.</param>
        protected void AddRule (string ruleName, IBusinessRule<TEntity> rule)
        {
            Guard.Against<ArgumentNullException>(rule == null,
                                                 "Cannot add a null rule instance. Expected a non null reference.");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(ruleName),
                                                 "Cannot add a rule with an empty or null rule name.");
            Guard.Against<ArgumentException>(_ruleSets.ContainsKey(ruleName),
                                             "Another rule with the same name already exists. Cannot add duplicate rules.");

            _ruleSets.Add(ruleName, rule);
        }

        /// <summary>
        /// Removes a previously added rule, specified with the <paramref name="ruleName"/>, from the evaluator.
        /// </summary>
        /// <param name="ruleName">string. The name of the rule to remove.</param>
        protected void RemoveRule (string ruleName)
        {
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(ruleName), "Expected a non empty and non-null rule name.");
            _ruleSets.Remove(ruleName);
        }

        /// <summary>
        /// Evaluates all business rules registred with the evaluator against a entity instance.
        /// </summary>
        /// <param name="entity">The <typeparamref name="TEntity"/> instance against which all 
        /// registered business rules are evauluated.</param>
        public void Evauluate(TEntity entity)
        {
            Guard.Against<ArgumentNullException>(entity == null,
                                                 "Cannot evaluate rules against a null reference. Expected a valid non-null entity instance.");
            _ruleSets.Keys.ForEach(x => EvaluateRule(x, entity));
        }

        /// <summary>
        /// Evaluates a business rules against an entity.
        /// </summary>
        /// <param name="ruleName">string. The name of the rule to evaluate.</param>
        /// <param name="entity">A <typeparamref name="TEntity"/> instance against which the business rules are evaluated.</param>
        private void EvaluateRule(string ruleName, TEntity entity)
        {
            Guard.Against<ArgumentNullException>(entity == null, "Cannot evaluate a business rule set against a null reference.");
            if (_ruleSets.ContainsKey(ruleName))
            {
                _ruleSets[ruleName].Evaluate(entity);
            }
        }
    }
}
