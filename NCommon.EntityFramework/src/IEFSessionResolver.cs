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
using System.Data.Objects;

namespace NCommon.Data.EntityFramework
{
    /// <summary>
    /// Interface implemented by a custom resolver for Entity Framework that resolves <see cref="ObjectContext"/>
    /// instances for a type.
    /// </summary>
    public interface IEFSessionResolver
    {
        /// <summary>
        /// Gets the unique <see cref="IEFSession"/> key for a type. 
        /// </summary>
        /// <typeparam name="T">The type for which the ObjectContext key should be retrieved.</typeparam>
        /// <returns>A <see cref="Guid"/> representing the unique object context key.</returns>
        Guid GetSessionKeyFor<T>();

        /// <summary>
        /// Opens a <see cref="IEFSession"/> instance for a given type.
        /// </summary>
        /// <typeparam name="T">The type for which an <see cref="IEFSession"/> is returned.</typeparam>
        /// <returns>An instance of <see cref="IEFSession"/>.</returns>
        IEFSession OpenSessionFor<T>();

        /// <summary>
        /// Gets the <see cref="ObjectContext"/> that can be used to query and update a given type.
        /// </summary>
        /// <typeparam name="T">The type for which an <see cref="ObjectContext"/> is returned.</typeparam>
        /// <returns>An <see cref="ObjectContext"/> that can be used to query and update the given type.</returns>
        ObjectContext GetObjectContextFor<T>();

        /// <summary>
        /// Registers an <see cref="ObjectContext"/> provider with the resolver.
        /// </summary>
        /// <param name="contextProvider">A <see cref="Func{T}"/> of type <see cref="ObjectContext"/>.</param>
        void RegisterObjectContextProvider(Func<ObjectContext> contextProvider);

        /// <summary>
        /// Gets the count of <see cref="ObjectContext"/> providers registered with the resolver.
        /// </summary>
        int ObjectContextsRegistered { get; }
    }
}