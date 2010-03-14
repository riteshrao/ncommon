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
using System.Data.Common;
using System.Data.Linq;

namespace NCommon.Data.LinqToSql
{
    /// <summary>
    /// Defines an interface that wraps a <see cref="DataContext"/> instance.
    /// </summary>
    /// <remarks>
    /// Since it's difficut to actually mock a DataContext and its Connection and Transaction
    /// properties, to facillitate testing, the ILinqSession interface is used to actually
    /// wrap the underlying data context.
    /// 
    /// You should never have to use this interface and it's default implementation but it's set as public
    /// access incase there is ever a requirement to provide specialized implementations of 
    /// ILinqSession instances.
    /// </remarks>
    public interface ILinqToSqlSession : IDisposable
    {
        /// <summary>
        /// Gets the actual <see cref="DataContext"/> that the interface wraps.
        /// </summary>
        DataContext Context { get; }
        
        /// <summary>
        /// Gets the <see cref="IDbConnection"/> that is used by the underlying DataContext.
        /// </summary>
        IDbConnection Connection { get;}

        /// <summary>
        /// Gets or sets the <see cref="IDbTransaction"/> to use with the data context.
        /// </summary>
        IDbTransaction Transaction { get; set;}

        /// <summary>
        /// Submits the changes in the underlying data context.
        /// </summary>
        void SubmitChanges();
    }

    /// <summary>
    /// Internal implementation of the <see cref="ILinqToSqlSession"/> interface
    /// </summary>
    internal class LinqToSqlSession : ILinqToSqlSession
    {
        private bool _disposed;
        private DataContext _context;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="LinqToSqlSession"/> class.
        /// </summary>
        public LinqToSqlSession(DataContext context)
        {
            Guard.Against<ArgumentNullException>(context == null, "Expected a non-null DataContext instance.");
            _context = context;
        }

        /// <summary>
        /// Gets the actual <see cref="DataContext"/> that the interface wraps.
        /// </summary>
        public DataContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Gets the <see cref="IDbConnection"/> that is used by the underlying DataContext.
        /// </summary>
        public IDbConnection Connection
        {
            get { return _context.Connection; }
        }

        /// <summary>
        /// Gets or sets the <see cref="IDbTransaction"/> to use with the data context.
        /// </summary>
        public IDbTransaction Transaction
        {
            get { return _context.Transaction; }
            set { _context.Transaction = (DbTransaction) value; }
        }

        /// <summary>
        /// Submits the changes in the underlying data context.
        /// </summary>
        public void SubmitChanges()
        {
            _context.SubmitChanges();
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
        /// Disposes off the managed and unmanaged resources used.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_disposed)
                {
                    _context.Dispose();
                    _disposed = true;
                }
            }
        }
    }
}
