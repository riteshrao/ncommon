using System.Collections.Generic;

namespace NCommon.Data.NHibernate.Tests.HRDomain.Domain
{
    public class SalesManager : Employee
    {
        ICollection<Employee> _subordinates = new HashSet<Employee>();

        public virtual IEnumerable<Employee> Subordinates
        {
            get { return _subordinates; }
        }
    }
}