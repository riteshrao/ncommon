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
    /// Defines an interface implemented by a business rule evaluator for an entity.
    /// </summary>
    /// <typeparam name="TEntity">The entity type that the business rules are applicable for.</typeparam>
    public interface IBusinessRulesEvaluator<TEntity>
    {
        /// <summary>
        /// Evaluates a business rules against an entity.
        /// </summary>
        /// <param name="entity">A <typeparamref name="TEntity"/> instance against which the business
        /// rules are evaluated.</param>
        void Evauluate(TEntity entity);
    }
}
