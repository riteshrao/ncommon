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
using System.Data;
using System.Data.Objects;

namespace NCommon.Data.EntityFramework
{
    /// <summary>
    /// Defines an interface that wraps a <see cref="ObjectContext"/> instance.
    /// </summary>
    /// <remarks>
    /// Since it's difficut to actually mock a ObjectContext and its Connection property,
    /// to facillitate testing, the IEFSession interface is used to actually
    /// wrap the underlying data context.
    /// 
    /// You should never have to use this interface and it's default implementation but it's set as public
    /// access incase there is ever a requirement to provide specialized implementations of 
    /// IEFSession instances.
    /// </remarks>
    public interface IEFSession : IDisposable
    {
        /// <summary>
        /// Gets the underlying <see cref="ObjectContext"/>
        /// </summary>
        ObjectContext Context { get; }

        /// <summary>
        /// Gets the Connection used by the <see cref="ObjectContext"/>
        /// </summary>
        IDbConnection Connection { get;}

        /// <summary>
        /// Adds a transient instance to the context associated with the session.
        /// </summary>
        /// <param name="entity"></param>
        void Add<T>(T entity) where T : class;

        /// <summary>
        /// Deletes an entity instance from the context.
        /// </summary>
        /// <param name="entity"></param>
        void Delete<T>(T entity) where T : class;

        /// <summary>
        /// Attaches an entity to the context. Changes to the entityt will be tracked by the underlying <see cref="ObjectContext"/>
        /// </summary>
        /// <param name="entity"></param>
        void Attach<T>(T entity) where T : class;

        /// <summary>
        /// Detaches an entity from the context. Changes to the entity will not be tracked by the underlying <see cref="ObjectContext"/>.
        /// </summary>
        /// <param name="entity"></param>
        void Detach<T>(T entity)  where T : class;

        /// <summary>
        /// Refreshes an entity.
        /// </summary>
        /// <param name="entity"></param>
        void Refresh<T>(T entity) where T : class;

        /// <summary>
        /// Creates an <see cref="ObjectQuery"/> of <typeparamref name="T"/> that can be used
        /// to query the entity.
        /// </summary>
        /// <typeparam name="T">The entityt type to query.</typeparam>
        /// <returns>A <see cref="ObjectQuery{T}"/> instance.</returns>
        ObjectQuery<T> CreateQuery<T>() where T : class;

        /// <summary>
        /// Saves changes made to the object context to the database.
        /// </summary>
        void SaveChanges();
    }
}
