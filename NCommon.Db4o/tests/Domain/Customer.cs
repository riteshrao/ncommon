using System.Collections.Generic;

namespace NCommon.Data.Db4o.Tests.Domain
{
    public class Customer
    {
        public virtual int CustomerID { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual Address Address { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}