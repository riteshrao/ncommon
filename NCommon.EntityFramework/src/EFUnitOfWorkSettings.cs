using System.Data;

namespace NCommon.Data.EntityFramework
{
    public class EFUnitOfWorkSettings
    {
        public IsolationLevel DefaultIsolationLevel { get; set; }
        public IEFSessionResolver SessionResolver { get; set; }
    }
}