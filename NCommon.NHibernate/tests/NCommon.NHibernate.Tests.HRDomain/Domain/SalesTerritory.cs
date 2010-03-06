using System;
using System.Collections.Generic;

namespace NCommon.Data.NHibernate.Tests.HRDomain.Domain
{
    public class SalesTerritory
    {
        ICollection<SalesPerson> _salesPersons = new HashSet<SalesPerson>();

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual IEnumerable<SalesPerson> SalesPersons
        {
            get { return _salesPersons;}
        }
    }
}