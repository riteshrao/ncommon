#region license
//Copyright 2008 Ritesh Rao 

//Licensed under the Apache License, Version 2.0 (the "License"); 
//you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 

//http://www.apache.org/licenses/LICENSE-2.0 

//Unless required by applicable law or agreed to in writing, software 
//distributed under the License is distributed on an "AS IS" BASIS, 
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and 
//limitations under the License. 
#endregion

using System;
using System.Collections.Generic;
using NCommon.Extensions;

namespace NCommon.Db4o.Tests.Domain
{
    public class Order
    {
        public Order()
        {
            Items = new HashSet<OrderItem>();   
        }
        public virtual int OrderID { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual DateTime OrderDate { get; set; }
        public virtual DateTime ShipDate { get; set; }
        public virtual ICollection<OrderItem> Items { get; set; }

        /// <summary>
        /// Simple method to calculate total of all items in the order.
        /// </summary>
        /// <returns></returns>
        public virtual decimal CalculateTotal ()
        {
            decimal total = 0;
            Items.ForEach(x => total += x.TotalPrice);
            return total;
        }
    }
}