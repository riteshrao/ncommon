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
using NCommon.DataServices.Transactions;
using NCommon.Extensions;
using NHibernate;

namespace NCommon.Data.NHibernate
{
    /// <summary>
    /// Implements the <see cref="IUnitOfWork"/> interface to provide an implementation
    /// of a IUnitOfWork that uses NHibernate to query and update the underlying store.
    /// </summary>
    public class NHUnitOfWork : IUnitOfWork
    {
        bool _disposed;
        readonly INHSessionResolver _sessionResolver;
        IDictionary<Guid, ISession> _openSessions = new Dictionary<Guid, ISession>();

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="NHUnitOfWork"/> that uses the provided
        /// NHibernate <see cref="ISession"/> instance.
        /// </summary>
        /// <param name="sessionResolver">An instance of <see cref="NHUnitOfWorkSettings"/>.</param>
        public NHUnitOfWork(INHSessionResolver sessionResolver)
        {
            Guard.Against<ArgumentNullException>(sessionResolver == null,
                                                 "Expected a non-null instance of NHUnitOfWorkSettings.");
            _sessionResolver = sessionResolver;
        }

        /// <summary>
        /// Gets a <see cref="ISession"/> instance that can be used for querying and updating
        /// instances of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type for which a <see cref="ISession"/> is retrieved.</typeparam>
        /// <returns>An instance of <see cref="ISession"/> that can be used for querying and updating
        /// instances of <typeparamref name="T"/></returns>
        public ISession GetSession<T>()
        {
            var sessionKey = _sessionResolver.GetSessionKeyFor<T>();
            if (_openSessions.ContainsKey(sessionKey))
                return _openSessions[sessionKey];

            //Opening a new session...
            var session = _sessionResolver.OpenSessionFor<T>();
            _openSessions.Add(sessionKey, session);
            return session;
        }

        /// <summary>
        /// Flushes the changes made in the unit of work to the data store.
        /// </summary>
        public void Flush()
        {
            _openSessions.ForEach(session => session.Value.Flush());
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
        /// Disposes off managed resources used by the NHUnitOfWork instance.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (_disposed) return;

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
