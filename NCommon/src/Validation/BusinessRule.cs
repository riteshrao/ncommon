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
    /// Implements the <see cref="IBusinessRule{TEntity}"/> interface and inherits from the
    /// <see cref="SpecificationRuleBase{TEntity}"/> to provide a implementation of a business rule that
    /// uses specifications as rule logic.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BusinessRule<TEntity> : SpecificationRuleBase<TEntity>, IBusinessRule<TEntity> where TEntity : class
    {
        private readonly Action<TEntity> _action; //The business action to undertake.

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="BusinessRule{TEntity}"/> instance.
        /// </summary>
        /// <param name="rule">A <see cref="ISpecification{TEntity}"/> instance that acts as the underlying
        /// specification that this business rule is evaluated against.</param>
        /// <param name="action">A <see cref="Action{TEntity}"/> instance that is invoked when the business rule
        /// is satisfied.</param>
        public BusinessRule(ISpecification<TEntity> rule, Action<TEntity> action) : base(rule)
        {
            Guard.Against<ArgumentNullException>(action == null, "Please provide a valid non null Action<TEntity> delegate instance.");
            _action = action;
        }

        /// <summary>
        /// Evaluates the business rule against an entity instance.
        /// </summary>
        /// <param name="entity"><typeparamref name="TEntity"/>. The entity instance against which
        /// the business rule is evaluated.</param>
        public void Evaluate(TEntity entity)
        {
            Guard.Against<ArgumentNullException>(entity == null,
                                                 "Cannot evaulate a business rule against a null reference.");
            if (IsSatisfied(entity))
                _action(entity);
        }
    }
}
