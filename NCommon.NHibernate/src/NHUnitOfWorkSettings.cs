using System.Data;

namespace NCommon.Data.NHibernate
{
    public class NHUnitOfWorkSettings
    {
        public IsolationLevel DefaultIsolation { get; set; }
        public INHSessionResolver SessionResolver { get; set; }
    }
}