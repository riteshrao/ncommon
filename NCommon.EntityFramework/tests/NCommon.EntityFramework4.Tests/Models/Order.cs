using System;
using System.Collections.Generic;

namespace NCommon.EntityFramework4.Tests.Models
{
    public class Order
    {
        ICollection<OrderItem> _orderItems;

        public virtual int OrderID { get; set; }
        public virtual int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual DateTime OrderDate { get; set; }
        public virtual DateTime ShipDate { get; set; }
        public virtual ICollection<OrderItem> OrderItems
        {
            get { return _orderItems ?? (_orderItems = new HashSet<OrderItem>());}
            set { _orderItems = value; }
        }
    }
}