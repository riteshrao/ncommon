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

namespace NCommon.Events
{
    /// <summary>
    /// Interface used by handlers of domain events.
    /// </summary>
    /// <typeparam name="T">A type of <see cref="DomainEvent"/></typeparam>
    public interface Handles<T> where T : IDomainEvent
    {
        /// <summary>
        /// Method invoked when a domain event of <typeparamref name="T"/> is raised.
        /// </summary>
        /// <param name="event"></param>
        void Handle(T @event);
    }
}