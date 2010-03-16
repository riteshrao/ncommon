using System.Data;

namespace NCommon.Data.EntityFramework
{
    /// <summary>
    /// Contains settings for <see cref="EFUnitOfWork"/> instances.
    /// </summary>
    public class EFUnitOfWorkSettings
    {
        public IsolationLevel DefaultIsolation { get; set; }
        public IEFSessionResolver SessionResolver { get; set; }
    }
}