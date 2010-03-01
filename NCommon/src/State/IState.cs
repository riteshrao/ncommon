namespace NCommon.State
{
    /// <summary>
    /// Base IState interface.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Gets the application specific state.
        /// </summary>
        IApplicationState Application { get; }
        /// <summary>
        /// Gets the session specific state.
        /// </summary>
        ISessionState Session { get; }
        /// <summary>
        /// Gets the cache specific state.
        /// </summary>
        ICacheState Cache { get; }
        /// <summary>
        /// Gets the thread local / request local specific state.
        /// </summary>
        ILocalState Local { get; }
    }
}