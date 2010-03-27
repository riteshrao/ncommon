namespace NCommon.Data
{
    /// <summary>
    /// Implemented by a transaction manager that manages unit of work transactions.
    /// </summary>
    public interface ITransactionManager
    {
        /// <summary>
        /// Returns the current <see cref="IUnitOfWork"/>.
        /// </summary>
        IUnitOfWork CurrentUnitOfWork { get;}
        /// <summary>
        /// Enlists a <see cref="UnitOfWorkScope"/> instance with the transaction manager.
        /// </summary>
        /// <param name="scope">bool. True if the scope should be enlisted in a new transaction, else
        /// false if the scope should participate in the existing transaction</param>
        /// <param name="newTransaction"></param>
        void EnlistScope(IUnitOfWorkScope scope, bool newTransaction);
    }
}