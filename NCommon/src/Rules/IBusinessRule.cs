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


namespace NCommon.Rules
{
    /// <summary>
    /// An interface that defines business rule for an entity instance.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity that this business rule evaluates.</typeparam>
    public interface IBusinessRule <TEntity>
    {
        /// <summary>
        /// Evaluates the business rule against an entity instance.
        /// </summary>
        /// <param name="entity"><typeparamref name="TEntity"/>. The entity instance against which
        /// the business rule is evaluated.</param>
        void Evaluate(TEntity entity);
    }
}
