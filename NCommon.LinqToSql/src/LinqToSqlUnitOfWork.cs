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
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Linq;
using NCommon.Extensions;

namespace NCommon.Data.LinqToSql
{
    /// <summary>
    /// Implements the <see cref="IUnitOfWork"/> interface to provide an implementation
    /// of a IUnitOfWork that uses NHibernate to query and update the underlying store.
    /// </summary>
    public class LinqToSqlUnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        readonly LinqToSqlUnitOfWorkSettings _settings;
        IDictionary<Guid, ILinqToSqlSession> _openSessions = new Dictionary<Guid, ILinqToSqlSession>();

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="LinqToSqlUnitOfWork"/> class that uses the specified data  context.
        /// </summary>
        /// <param name="context">The <see cref="DataContext"/> instance that the LinqToSqlUnitOfWork instance uses.</param>
        public LinqToSqlUnitOfWork(LinqToSqlUnitOfWorkSettings settings) 
        {
            Guard.Against<ArgumentNullException>(settings == null,
                                                 "Expected a non-null LinqToSqlUnitOfWorkSettings class.");
            _settings = settings;
        }

        /// <summary>
        /// Gets a <see cref="ILinqToSqlSession"/> that can be used to query and update the specified type.
        /// </summary>
        /// <typeparam name="T">The type for which a <see cref="ILinqToSqlSession"/> instance is retrieved.</typeparam>
        /// <returns>A <see cref="ILinqToSqlSession"/> instance that can be used to query and update the specified type.</returns>
        public ILinqToSqlSession GetSession<T>()
        {
            var key = _settings.SessionResolver.GetSessionKeyFor<T>();
            if (_openSessions.ContainsKey(key))
                return _openSessions[key];

            //Opening a new session...
            var session = _settings.SessionResolver.OpenSessionFor<T>();
            _openSessions.Add(key, session);
            return session;
        }

        /// <summary>
        /// Flushes the changes made in the unit of work to the data store.
        /// </summary>
        public void Flush()
        {
            _openSessions.ForEach(session => session.Value.SubmitChanges());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes off manages resources used by the LinqToSqlUnitOfWork instance.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (_disposed) 
                return;

            if (disposing)
            {
                if (_openSessions != null && _openSessions.Count > 0)
                {
                    _openSessions.ForEach(session => session.Value.Dispose());
                    _openSessions.Clear();
                }    
            }
            _openSessions = null;
            _disposed = true;
        }
    }
}
