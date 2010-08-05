namespace NCommon.Data
{
    /// <summary>
    /// Defines the transaction mode when creating a new <see cref="UnitOfWorkScope"/> instance.
    /// </summary>
    public enum TransactionMode
    {
        /// <summary>
        /// Specifies that the <see cref="UnitOfWorkScope"/> should be created using default
        /// transaction mode.
        /// </summary>
        /// <remarks>
        /// The default transaction mode instructs the <see cref="UnitOfWorkScope"/> to enlist in
        /// a parent <see cref="UnitOfWorkScope"/>'s transaction, or if one doesnt exist, then
        /// creates a new transaction.
        /// </remarks>
        Default = 0,
        /// <summary>
        /// Specifies that the scope should not participate in a parent <see cref="UnitOfWorkScope"/>'s transaction,
        /// if one exists, and should start it's own transaction.
        /// </summary>
        New = 1,
        /// <summary>
        /// Specifies that the <see cref="UnitOfWorkScope"/> should not participate in a parent scope's transaction,
        /// and should not start a transaction of its own.
        /// </summary>
        /// <remarks>
        /// If a scope is created using the Supress option, any child scopes created with the default 
        /// transaction mode, i.e. <see cref="Default"/> will also not participate in any transaction, although
        /// it will share the same parent <see cref="IUnitOfWork"/> instance.
        /// </remarks>
        Supress = 2
    }
}