using System.Transactions;

namespace NCommon.Data
{
    /// <summary>
    /// Helper class to create <see cref="TransactionScope"/> instances.
    /// </summary>
    public static class TransactionScopeHelper
    {
        /// <summary>
        /// Creates a <see cref="TransactionScope"/> with the specified isolation level.
        /// </summary>
        /// <param name="isolationLevel">The <see cref="IsolationLevel"/> of the scope.</param>
        /// <returns>A <see cref="TransactionScope"/> instance.</returns>
        /// <remarks>If an ambient transaction with the same isolation level exists, this method
        /// will create a new instance of <see cref="TransactionScope"/> that is part of the ambient
        /// transaction, else it will create a new scope with the specified isolation level.</remarks>
        public static TransactionScope CreateScope(IsolationLevel isolationLevel)
        {
            if (Transaction.Current == null ||
                Transaction.Current.IsolationLevel != isolationLevel)
                return CreateNewScope(isolationLevel);
            return new TransactionScope(Transaction.Current);
        }

        /// <summary>
        /// Creates a <see cref="TransactionScope"/> with the specified isolation level and
        /// does not enlist as part of an existing ambient transaction.
        /// </summary>
        /// <param name="isolationLevel">The <see cref="IsolationLevel"/> of the scope.</param>
        /// <returns>An instance of <see cref="TransactionScope"/>.</returns>
        public static TransactionScope CreateNewScope(IsolationLevel isolationLevel)
        {
            return new TransactionScope(TransactionScopeOption.RequiresNew,
                                        new TransactionOptions {IsolationLevel = isolationLevel});
        }
    }
}