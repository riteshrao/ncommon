using System.Data;

namespace NCommon.Data.NHibernate
{
    /// <summary>
    /// Contains settings for <see cref="NHUnitOfWork"/> instances.
    /// </summary>
    public class NHUnitOfWorkSettings
    {
        public IsolationLevel DefaultIsolation { get; set; }
        public INHSessionResolver SessionResolver { get; set; }
    }
}