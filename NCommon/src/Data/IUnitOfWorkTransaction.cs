using System;

namespace NCommon.Data
{
    ///<summary>
    /// An interface implemented by <see cref="IUnitOfWork"/> transactions.
    ///</summary>
    public interface IUnitOfWorkTransaction : IDisposable
    {
        /// <summary>
        /// Signalls a commit on the transction.
        /// </summary>
        void Commit();
        /// <summary>
        /// Signals a rollback on the transaction.
        /// </summary>
        void Rollback();
    }
}