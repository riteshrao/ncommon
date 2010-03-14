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
using NCommon.Specifications;

namespace NCommon.Rules
{
    /// <summary>
    /// Base implementation that uses <see cref="ISpecification{TEntity}"/> instances that provide the logic to check if the
    /// rule is satisfied.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SpecificationRuleBase<TEntity>
    {
        private readonly ISpecification<TEntity> _rule; //The underlying rule as a specification.

        /// <summary>
        /// Default Constructor. 
        /// Protected. Must be called by implementors.
        /// </summary>
        /// <param name="rule">A <see cref="ISpecification{TEntity}"/> instance that specifies the rule.</param>
        protected SpecificationRuleBase(ISpecification<TEntity> rule)
        {
            Guard.Against<ArgumentNullException>(rule == null, "Expected a non null and valid ISpecification<TEntity> rule instance.");
            _rule = rule;
        }

        /// <summary>
        /// Checks if the entity instance satisfies this rule.
        /// </summary>
        /// <param name="entity">The <typeparamref name="TEntity"/> insance.</param>
        /// <returns>bool. True if the rule is satsified, else false.</returns>
        public bool IsSatisfied(TEntity entity)
        {
            Guard.Against<ArgumentNullException>(entity == null,
                                                 "Expected a valid non-null entity instance against which the rule can be evaluated.");
            return _rule.IsSatisfiedBy(entity);
        }
    }
}
