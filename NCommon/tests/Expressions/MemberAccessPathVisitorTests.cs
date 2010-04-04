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
using System.Linq.Expressions;
using NUnit.Framework;

namespace NCommon.Expressions.Tests
{
    /// <summary>
    /// Test the <see cref="MemberAccessPathVisitor"/> class.
    /// </summary>
    [TestFixture]
    public class MemberAccessPathVisitorTests
    {
        #region Test Classes
        public class SalesPerson
        {
            public Customer PrimaryCustomer { get; set; }

            public object MethodAccess() {return null;}
        }

        public class Customer {public IList<Order> Orders;}

        public class Order {}
        #endregion

        [Test]
        public void Visit_Customer_Orders_Should_Return_Orders_As_Path ()
        {
            Expression<Func<Customer, object>> expression = x => x.Orders;
            var visitor = new MemberAccessPathVisitor();
            visitor.Visit(expression);
            Assert.That(visitor.Path, Is.EqualTo("Orders"));
        }

        [Test]
        public void Visit_SalesPerson_Customer_Orders_Should_Return_Customer_Orders_As_Path()
        {
            Expression<Func<SalesPerson, object>> expression = x => x.PrimaryCustomer.Orders;
            var visitor = new MemberAccessPathVisitor();
            visitor.Visit(expression);
            Assert.That(visitor.Path, Is.EqualTo("PrimaryCustomer.Orders"));
        }

        [Test]
        public void Visit_Throws_NotSupportedException_When_Expression_Contains_Method_Call()
        {
            Expression<Func<SalesPerson, object>> expression = x => x.MethodAccess();
            var visitor = new MemberAccessPathVisitor();
            Assert.Throws<NotSupportedException>(() => visitor.Visit(expression));
        }
    }

    
}
