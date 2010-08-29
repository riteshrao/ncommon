using System.Collections.Generic;

namespace NCommon.EntityFramework4.Tests.Models
{
    public class Customer
    {
        ICollection<Order> _orders;

        public virtual int CustomerID { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string StreetAddress1 { get; set; }
        public virtual string StreetAddress2 { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string ZipCode { get; set; }
        public virtual ICollection<Order> Orders 
        {
            get { return _orders ?? (_orders = new HashSet<Order>()); }
            set { _orders = value; }
        }
    }
}