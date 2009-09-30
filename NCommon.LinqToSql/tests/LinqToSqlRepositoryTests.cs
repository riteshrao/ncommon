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
using System.Linq;
using System.Transactions;
using Microsoft.Practices.ServiceLocation;
using NCommon.Extensions;
using NCommon.LinqToSql.Tests;
using NCommon.Specifications;
using NUnit.Framework;
using Rhino.Mocks;
using IsolationLevel=System.Data.IsolationLevel;

namespace NCommon.Data.LinqToSql.Tests

{
    /// <summary>
    /// Tests the <see cref="LinqToSqlRepository{TEntity}"/> class
    /// </summary>
    [TestFixture]
    public class LinqToSqlRepositoryTests
    {
        #region setup
        /// <summary>
        /// Sets up the NHibernate SessionFactory and NHUnitOfWorkFactory.
        /// </summary>
        [TestFixtureSetUp]
        public void SetUp()
        {
            LinqToSqlUnitOfWorkFactory.SetDataContextProvider(() => 
            { 
                var context = new TestDataDataContext {DeferredLoadingEnabled = true};
                //context.Log = Console.Out; //Uncomment if you want to see SQL output.
                return context;
            });
            var locator = MockRepository.GenerateStub<IServiceLocator>();
            locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>())
                   .Return(new LinqToSqlUnitOfWorkFactory()).Repeat.Any();
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            LinqToSqlUnitOfWorkFactory.SetDataContextProvider(null);
        }
        #endregion

        #region tests
        [Test]
        public void Query_With_No_UnitOfWork_Throws_InvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                                                         {
                                                             var repository = new LinqToSqlRepository<Customer>{new Customer()};
                                                         });
        }

        [Test]
        public void Query_With_Incompatible_UnitOfWork_Throws_InvalidOperationException()
        {
            IUnitOfWork mockUnitOfWork = MockRepository.GenerateStub<IUnitOfWork>();
            UnitOfWork.Current = mockUnitOfWork;
            Assert.Throws<InvalidOperationException>(() =>
            {
                var customerRepository = new LinqToSqlRepository<Customer>();
                var results = from customer in customerRepository
                              select customer;
            }
                );
            UnitOfWork.Current = null;
        }

        [Test]
        public void Query_Allows_Lazy_Load_While_UnitOfWork_Still_Running()
        {
            using (new UnitOfWorkScope())
            {
                var ordersRepository = new LinqToSqlRepository<Order>();
                var results = from order in ordersRepository
                              select order;

                Assert.DoesNotThrow
                (
                    () => results.ForEach
                    (
                        x => Assert.That(!string.IsNullOrEmpty(x.Customer.FirstName))
                    )
                );

            }
        }

        [Test]
        public void Query_Allows_Eger_Loading_Using_With()
        {
            using (new UnitOfWorkScope())
            {
                var ordersRepository = new LinqToSqlRepository<Order>();
				var results = from order in ordersRepository.With(x => x.Customer)
                              select order;

                Assert.DoesNotThrow(() => results.ForEach(x =>
                {
                    Assert.That(x.Customer, Is.TypeOf(typeof(Customer)));
                    Assert.That(!string.IsNullOrEmpty(x.Customer.FirstName));
                }));
            }
        }

        [Test]
        public void Query_Allows_Projection_Using_Select_Projection()
        {
            using (new UnitOfWorkScope())
            {
                var ordersRepository = new LinqToSqlRepository<Order>();
                var results = from order in ordersRepository
                              select new { order.Customer.FirstName, order.Customer.LastName, order.ShipDate, order.OrderDate };

                Assert.DoesNotThrow(() => results.ForEach(x =>
                {
                    Assert.That(!string.IsNullOrEmpty(x.LastName));
                    Assert.That(!string.IsNullOrEmpty(x.FirstName));
                }));

            }
        }

        [Test]
        public void Query_Throws_Exception_When_LazyLoading_After_UnitOfWork_Is_Finished()
        {
            Customer customer;
            using (new UnitOfWorkScope())
            {
                var customerRepository = new LinqToSqlRepository<Customer>();
                customer = (from cust in customerRepository select cust).FirstOrDefault();
            }
            Assert.That(customer, Is.Not.Null);
            Assert.Throws<ObjectDisposedException>(() => customer.Orders.Count());
        }

        [Test]
        public void Save_New_Customer_Saves_Customer_When_UnitOfWork_Is_Committed()
        {
            Random rnd = new Random();
            var newCustomer = new Customer
            {
                FirstName = ("John_" + rnd.Next(0, 30000)),
                LastName = ("Doe_" + rnd.Next(0, 30000)),
                StreetAddress1 = "This record was inserted via a test",
                City = "Fictional city",
                State = "LA",
                ZipCode = "12345"
            };

            var queryForCustomer = new Func<LinqToSqlRepository<Customer>, Customer>
                (
                    x => (from cust in x
                          where cust.FirstName == newCustomer.FirstName && cust.LastName == newCustomer.LastName
                          select cust).FirstOrDefault()
                );

            using (var scope = new UnitOfWorkScope())
            {
                var customerRepository = new LinqToSqlRepository<Customer>();
                var recordCheckResult = queryForCustomer(customerRepository);
                Assert.That(recordCheckResult, Is.Null);

                customerRepository.Add(newCustomer);
                scope.Commit();
            }

            //Starting a completely new unit of work and repository to check for existance.
            using (var scope = new UnitOfWorkScope())
            {
                var customerRepository = new LinqToSqlRepository<Customer>();
                var recordCheckResult = queryForCustomer(customerRepository);
                Assert.That(recordCheckResult, Is.Not.Null);
                Assert.That(recordCheckResult.FirstName, Is.EqualTo(newCustomer.FirstName));
                Assert.That(recordCheckResult.LastName, Is.EqualTo(newCustomer.LastName));
                scope.Commit();
            }
        }

        [Test]
        public void Save_Does_Not_Save_New_Customer_When_UnitOfWork_Is_Aborted()
        {
            Random rnd = new Random();
            var newCustomer = new Customer
            {
                FirstName = ("John_" + rnd.Next(30001, 50000)),
                LastName = ("Doe_" + rnd.Next(30001, 50000)),
                StreetAddress1 = "This record was inserted via a test",
                City = "Fictional city",
                State = "LA",
                ZipCode = "12345"
            };

            using (new UnitOfWorkScope())
            {
                var customerRepository = new LinqToSqlRepository<Customer>();
                var recordCheckResult = (from cust in customerRepository
                                         where cust.FirstName == newCustomer.FirstName &&
                                               cust.LastName == newCustomer.LastName
                                         select cust).FirstOrDefault();
                Assert.That(recordCheckResult, Is.Null);

                customerRepository.Add(newCustomer);
                //DO NOT CALL COMMIT TO SIMMULATE A ROLLBACK.
            }

            //Starting a completely new unit of work and repository to check for existance.
            using (var scope = new UnitOfWorkScope())
            {
                var customerRepository = new LinqToSqlRepository<Customer>();
                var recordCheckResult = (from cust in customerRepository
                                         where cust.FirstName == newCustomer.FirstName &&
                                               cust.LastName == newCustomer.LastName
                                         select cust).FirstOrDefault();
                Assert.That(recordCheckResult, Is.Null);
                scope.Commit();
            }
        }

        [Test]
        public void Save_Updates_Existing_Order_Record()
        {
            int orderIDRetrieved;
            var updatedDate = DateTime.Now;
            var randomOrderNo = new Random();

            using (var scope = new UnitOfWorkScope())
            {
                var orderRepository = new LinqToSqlRepository<Order>();
                var order = (from o in orderRepository
                             where o.OrderID == randomOrderNo.Next(1, 3)
                             select o).FirstOrDefault();

                Assert.That(order, Is.Not.Null);

                orderIDRetrieved = order.OrderID;
                order.OrderDate = updatedDate;

                scope.Commit();
            }

            using (new UnitOfWorkScope())
            {
                var orderRepository = new LinqToSqlRepository<Order>();
                var order = (from o in orderRepository
                             where o.OrderID == orderIDRetrieved
                             select o).FirstOrDefault();

                Assert.That(order, Is.Not.Null);
                Assert.That(order.OrderDate.Value.Date, Is.EqualTo(updatedDate.Date));
				Assert.That(order.OrderDate.Value.Hour, Is.EqualTo(updatedDate.Hour));
				Assert.That(order.OrderDate.Value.Minute, Is.EqualTo(updatedDate.Minute));
				Assert.That(order.OrderDate.Value.Second, Is.EqualTo(updatedDate.Second));
            }
        }

        [Test]
        public void Delete_Deletes_Record()
        {
            var newCustomer = new Customer
            {
                FirstName = ("John_DELETE_ME_" + DateTime.Now),
                LastName = ("Doe_DELETE_ME_" + DateTime.Now),
                StreetAddress1 = "This record was inserted for deletion",
                City = "Fictional city",
                State = "LA",
                ZipCode = "12345"
            };

            //Re-usable query to query for the matching record.
            var queryForCustomer = new Func<LinqToSqlRepository<Customer>, Customer>
                (
                    x => (from cust in x
                          where cust.FirstName == newCustomer.FirstName && cust.LastName == newCustomer.LastName
                          select cust).FirstOrDefault()
                );

            using (var scope = new UnitOfWorkScope())
            {
                var customerRepository = new LinqToSqlRepository<Customer>();
                var recordCheckResult = queryForCustomer(customerRepository);
                Assert.That(recordCheckResult, Is.Null);

                customerRepository.Add(newCustomer);
                scope.Commit();
            }

            //Retrieve the record for deletion.
            using (var scope = new UnitOfWorkScope())
            {
                var customerRepository = new LinqToSqlRepository<Customer>();
                var customerToDelete = queryForCustomer(customerRepository);
                Assert.That(customerToDelete, Is.Not.Null);
                customerRepository.Delete(customerToDelete);
                scope.Commit();
            }

            //Ensure customer record is deleted.
            using (new UnitOfWorkScope())
            {
                var customerRepository = new LinqToSqlRepository<Customer>();
                var recordCheckResult = queryForCustomer(customerRepository);
                Assert.That(recordCheckResult, Is.Null);
            }
        }

        [Test]
        public void Query_Using_QueryMethod_Returns_Matched_Records_Only()
        {
            using (new UnitOfWorkScope())
            {
                var customersInPA = new Specification<Order>(x => x.Customer.State == "PA");

                var ordersRepository = new LinqToSqlRepository<Order>();
                var results = from order in ordersRepository.Query(customersInPA) select order;

                Assert.That(results.Count() > 0);
            }
        }

        [Test]
        public void Query_Using_OrderBy_With_QueryMethod_Returns_Matched_Records_Only()
        {

            using (new UnitOfWorkScope())
            {
                var customersInPA = new Specification<Order>(x => x.Customer.State == "PA");

                var ordersRepository = new LinqToSqlRepository<Order>();
                var results = from order in ordersRepository.Query(customersInPA)
                              orderby order.OrderDate
                              select order;

                Assert.That(results.Count() > 0);
            }
        }

        [Test]
        public void When_Calling_CalculateTotal_On_Order_Returns_Valid_When_Under_UnitOfWork()
        {
            using (new UnitOfWorkScope())
            {
                var oredersRepository = new LinqToSqlRepository<Order>();
                var order = (from o in oredersRepository
                             select o).FirstOrDefault();

                Assert.That(order.CalculateTotal(), Is.GreaterThan(0));
            }
        }

        [Test]
        public void When_Calling_CalculateTotal_On_Order_Returns_Valid_With_No_UnitOfWork_Throws()
        {
            Order order;
            using (new UnitOfWorkScope())
            {
                var oredersRepository = new LinqToSqlRepository<Order>();
                order = (from o in oredersRepository
                         select o).FirstOrDefault();
            }
            Assert.Throws<ObjectDisposedException>(() => order.CalculateTotal());
        }

        [Test]
        public void When_No_FetchingStrategy_Registered_For_Makes_No_Changes()
        {
            Order order;
            using (new UnitOfWorkScope())
            {
                var oredersRepository = new LinqToSqlRepository<Order>();
                order = (from o in oredersRepository
                         select o).FirstOrDefault();
            }
            Assert.Throws<ObjectDisposedException>(() => order.CalculateTotal());
        }

        [Test]
        public void Repository_For_Uses_Registered_Fetching_Strategies()
        {
            IEnumerable<Order> orders;
            using (new UnitOfWorkScope())
            {
                var strategies = new IFetchingStrategy<Order, LinqToSqlRepositoryTests>[]
                                     {
                                             new Order_OrderItems_Strategy(),
                                             new OrderItems_Product_Strategy()
                                     };

                IRepository<Order> ordersRepository = null;
                ServiceLocator.Current.Expect(x => x.GetAllInstances<IFetchingStrategy<Order, LinqToSqlRepositoryTests>>())
                                        .Return(strategies);

                ordersRepository = new LinqToSqlRepository<Order>().For<LinqToSqlRepositoryTests>();
                orders = (from o in ordersRepository select o).ToList();
            }
            orders.ForEach(x => Assert.That(x.CalculateTotal(), Is.GreaterThan(0)));
        }

		[Test]
		public void UnitOfWork_is_rolledback_when_containing_TransactionScope_is_rolledback ()
		{
			int orderId;
			DateTime? oldDate;

			using (var txScope = new TransactionScope(TransactionScopeOption.Required))
			using (var uowScope = new UnitOfWorkScope(IsolationLevel.Serializable))
			{
				var ordersRepository = new LinqToSqlRepository<Order>();
				var order = (from o in ordersRepository
				             select o).First();

				oldDate = order.OrderDate;
				order.OrderDate = DateTime.Now;
				orderId = order.OrderID;
				uowScope.Commit();

				//Note: txScope has not been committed
			}

			using (var uowScope = new UnitOfWorkScope())
			{
				var ordersRepository = new LinqToSqlRepository<Order>();
				var order = (from o in ordersRepository
				             where o.OrderID == orderId 
							 select o).First();

				Assert.That(order.OrderDate, Is.EqualTo(oldDate));

			}
		}

    	private class Order_OrderItems_Strategy : IFetchingStrategy<Order, LinqToSqlRepositoryTests>
        {
            #region Implementation of IFetchingStrategy<Order,NHUnitOfWorkTests>
            ///<summary>
            /// Instructs the instance to define the fetching strategy on the repository instance.
            ///</summary>
            ///<param name="repository"></param>
            public void Define(IRepository<Order> repository)
            {
                repository.With(x => x.OrderItems);
            }
            #endregion
        }

        public class OrderItems_Product_Strategy : IFetchingStrategy<Order, LinqToSqlRepositoryTests>
        {
            #region Implementation of IFetchingStrategy<Order,NHUnitOfWorkTests>
            ///<summary>
            /// Instructs the instance to define the fetching strategy on the repository instance.
            ///</summary>
            ///<param name="repository"></param>
            public void Define(IRepository<Order> repository)
            {
                repository.With<OrderItem>(x => x.Product);
            }
            #endregion
        }
        #endregion
    }
}
