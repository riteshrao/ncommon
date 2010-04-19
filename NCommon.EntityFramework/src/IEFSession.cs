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
        /// Saves changes made to the object context to the database.
        /// </summary>
        void SaveChanges();
    }

    /// <summary>
    /// Internal implementation of the <see cref="IEFSession"/> interface.
    /// </summary>
    internal class EFSession : IEFSession
    {
        private bool _disposed;
        private ObjectContext _context;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="EFSession"/> class.
        /// </summary>
        /// <param name="context"></param>
        public EFSession(ObjectContext context)
        {
            Guard.Against<ArgumentNullException>(context == null, "Expected a non-null ObjectContext instance.");
            _context = context;
        }

        /// <summary>
        /// Gets the underlying <see cref="ObjectContext"/>
        /// </summary>
        public ObjectContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Gets the Connection used by the <see cref="ObjectContext"/>
        /// </summary>
        public IDbConnection Connection
        {
            get { return _context.Connection; }
        }

        /// <summary>
        /// Saves changes made to the object context to the database.
        /// </summary>
        public void SaveChanges()
        {
#if NETv40
            _context.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
#else
            _context.SaveChanges(true);
#endif
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
        /// Disposes off the managed and un-managed resources used.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!disposing) 
                return;
            if (_disposed) 
                return;

            _context.Dispose();
            _disposed = true;
        }
    }
}
