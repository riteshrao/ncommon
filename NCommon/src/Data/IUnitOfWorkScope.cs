using System;

namespace NCommon.Data
{
    ///<summary>
    ///</summary>
    public interface IUnitOfWorkScope : IDisposable
    {
        /// <summary>
        /// Event fired when the scope is comitting.
        /// </summary>
        event Action<IUnitOfWorkScope> ScopeComitting;

        /// <summary>
        /// Event fired when the scope is rollingback.
        /// </summary>
        event Action<IUnitOfWorkScope> ScopeRollingback;

        ///<summary>
        /// Commits the current running transaction in the scope.
        ///</summary>
        void Commit();

        /// <summary>
        /// Marks the scope as completed.
        /// Used for internally by NCommon and should not be used by consumers.
        /// </summary>
        void Complete();
    }
}