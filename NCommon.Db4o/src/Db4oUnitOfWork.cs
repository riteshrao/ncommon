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
using Db4objects.Db4o;

namespace NCommon.Data.Db4o
{
    /// <summary>
    /// Implements the <see cref="IUnitOfWork"/> interface to provide an implementation
    /// of a IUnitOfWork that uses Db4o to query and update the underlying store.
    /// </summary>
    public class Db4oUnitOfWork : IUnitOfWork
    {
        bool _disposed;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="IObjectContainer"/> class.
        /// </summary>
        /// <param name="container"></param>
        public Db4oUnitOfWork(IObjectContainer container)
        {
            Guard.Against<ArgumentNullException>(container == null,
                                                 "Expected a non-null IObjectContainer instance.");
            ObjectContainer = container;
        }

        /// <summary>
        /// Gets a <see cref="IObjectContainer"/> instance that can be used for querying and modifying
        /// the Db4o container.
        /// </summary>
        /// <returns>An instance of <see cref="IObjectContainer"/>.</returns>
        public IObjectContainer ObjectContainer { get; private set; }

        /// <summary>
        /// Flushes the changes made in the unit of work to the data store.
        /// </summary>
        public void Flush()
        {
            ObjectContainer.Commit();
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
            if (_disposed)
                return;

            if (disposing)
            {
                if (ObjectContainer != null)
                    ObjectContainer.Dispose();
            }
            ObjectContainer = null;
            _disposed = true;
        }
    }
}