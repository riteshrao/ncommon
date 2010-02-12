using System;
using Db4objects.Db4o;
using NCommon.Data;

namespace NCommon.Db4o
{
    /// <summary>
    /// Implementation of <see cref="ITransaction"/> interface for Db4o. 
    /// </summary>
    public class Db4oTransaction : ITransaction
    {
        bool _disposed;
        readonly IObjectContainer _contianer;

        public Db4oTransaction(IObjectContainer container)
        {
            Guard.Against<ArgumentNullException>(container == null, "Expected a non-null IObjectContainer instance.");
            _contianer = container;
        }

        public event EventHandler TransactionCommitted;
        public event EventHandler TransactionRolledback;

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        void dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing && _contianer != null)
                _contianer.Dispose();
            _disposed = true;
        }

        public void Commit()
        {
            if (_disposed)
                throw new ObjectDisposedException("Db4oTransaction", "Cannot commit a disposed transaction.");
            _contianer.Commit();
            if (TransactionCommitted != null)
                TransactionCommitted(this, EventArgs.Empty);
        }

        public void Rollback()
        {
            if (_disposed)
                throw new ObjectDisposedException("Db4oTransaction", "Cannot rollback a disposed transaction.");
            _contianer.Rollback();
            if (TransactionRolledback != null)
                TransactionRolledback(this, EventArgs.Empty);
        }
    }
}