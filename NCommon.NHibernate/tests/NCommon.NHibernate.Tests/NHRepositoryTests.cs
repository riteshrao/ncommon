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
using NCommon.Data.NHibernate.Tests.HRDomain.Domain;
using NCommon.Extensions;
using NCommon.Data.NHibernate.Tests.OrdersDomain;
using NCommon.Specifications;
using NHibernate;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.NHibernate.Tests
{
    /// <summary>
    /// Tests the <see cref="NHRepository{TEntity}"/> class.
    /// </summary>
    [TestFixture]
    public class NHRepositoryTests : NHTestBase
    {
        [Test]
        public void Delete_Deletes_Record()
        {
            //Adding a dummy record.
            var newAddress = new Address
            {
                StreetAddress1 = "This record was inserted for deletion",
                City = "Fictional city",
                State = "LA",
                ZipCode = "12345"
            };

            var newCustomer = new Customer
            {
                FirstName = ("John_DELETE_ME_" + DateTime.Now),
                LastName = ("Doe_DELETE_ME_" + DateTime.Now),
                Address = newAddress
            };

            //Re-usable query to query for the matching record.
            var queryForCustomer = new Func<NHRepository<Customer>, Customer>
                (x => (from cust in x
                       where cust.FirstName == newCustomer.FirstName && cust.LastName == newCustomer.LastName
                       select cust).FirstOrDefault()
                );

            using (var scope = new UnitOfWorkScope())
            {
                var customerRepository = new NHRepository<Customer>();
                var recordCheckResult = queryForCustomer(customerRepository);
                Assert.That(recordCheckResult, Is.Null);

                customerRepository.Add(newCustomer);
                scope.Commit();
            }

            //Retrieve the record for deletion.
            using (var scope = new UnitOfWorkScope())
            {
                var customerRepository = new NHRepository<Customer>();
                var customerToDelete = queryForCustomer(customerRepository);
                Assert.That(customerToDelete, Is.Not.Null);
                customerRepository.Delete(customerToDelete);
                scope.Commit();
            }

            //Ensure customer record is deleted.
            using (new UnitOfWorkScope())
            {
                var customerRepository = new NHRepository<Customer>();
                var recordCheckResult = queryForCustomer(customerRepository);
                Assert.That(recordCheckResult, Is.Null);
            }
        }

        [Test]
        public void Nested_UnitOfWork_With_Different_Transaction_Compatibility_Works()
        {
            var changedShipDate = DateTime.Now.AddDays(1);
            var changedOrderDate = DateTime.Now.AddDays(2);

            using (var testData = new NHTestDataGenerator(OrdersDomainFactory.OpenSession()))
            {
                testData.Batch(actions =>
                               actions.CreateOrderForCustomer(actions.CreateCustomer()));

                int orderId;
                using (new UnitOfWorkScope())
                {
                    var ordersRepository = new NHRepository<Order>();
                    orderId = ordersRepository.Select(x => x.OrderID).First();
                }

                Assert.NotNull(orderId);
                using (new UnitOfWorkScope())
                {
                    var outerRepository = new NHRepository<Order>();
                    var outerOrder = outerRepository.Where(x => x.OrderID == orderId).First();
                    outerOrder.OrderDate = changedOrderDate;

                    using (var innerScope = new UnitOfWorkScope(UnitOfWorkScopeOptions.CreateNew))
                    {
                        var innerRepository = new NHRepository<Order>();
                        var innerOrder = innerRepository.Where(x => x.OrderID == orderId).First();
                        innerOrder.ShipDate = changedShipDate;
                        innerScope.Commit();
                    }
                }

                using (new UnitOfWorkScope())
                {
                    var ordersRepository = new NHRepository<Order>();
                    var order = ordersRepository.First();
                    //NOTE: Not doing a time match here because time storage in DB is less precise than in memory.
                    Assert.That(order.OrderDate.Date, Is.Not.EqualTo(changedOrderDate.Date));
                    Assert.That(order.ShipDate.Date, Is.EqualTo(changedShipDate.Date));
                }
            }
        }

        [Test]
        public void Query_Allows_Eger_Loading_Using_With()
        {
            using (var testData = new NHTestDataGenerator(OrdersDomainFactory.OpenSession()))
            {
                testData.Batch(actions =>
                               actions.CreateOrderForCustomer(actions.CreateCustomer()));

                List<Order> results;
                using (new UnitOfWorkScope())
                {
                    var ordersRepository = new NHRepository<Order>();
                    results = (from order in ordersRepository.With(x => x.Customer)
                               select order).ToList();
                }
                Assert.DoesNotThrow(() => results.ForEach(x =>
                                                          Assert.That(NHibernateUtil.IsInitialized(x.Customer))));
            }
        }

        [Test]
        public void Query_Allows_Lazy_Load_While_UnitOfWork_Still_Running()
        {
            using (var testData = new NHTestDataGenerator(OrdersDomainFactory.OpenSession()))
            using (new UnitOfWorkScope())
            {
                testData.Batch(actions =>
                               actions.CreateOrderForCustomer(actions.CreateCustomer()));

                var ordersRepository = new NHRepository<Order>();
                var results = from order in ordersRepository
                              select order;

                Assert.DoesNotThrow(() => results.ForEach(x =>
                {
                    Assert.That(NHibernateUtil.IsInitialized(x.Customer), Is.False);
                    Assert.That(string.IsNullOrEmpty(x.Customer.FirstName), Is.False);
                }));
            }
        }

        [Test]
        public void Query_Allows_Projection_Using_Select_Projection()
        {
            using (var testData = new NHTestDataGenerator(OrdersDomainFactory.OpenSession()))
            using (new UnitOfWorkScope())
            {
                testData.Batch(actions =>
                               actions.CreateOrderForCustomer(actions.CreateCustomer()));

                var ordersRepository = new NHRepository<Order>();
                var results = from order in ordersRepository
                              select new
                              {
                                  order.Customer.FirstName,
                                  order.Customer.LastName,
                                  order.ShipDate,
                                  order.OrderDate
                              };

                Assert.DoesNotThrow(() => results.ForEach(x =>
                {
                    Assert.That(string.IsNullOrEmpty(x.LastName), Is.False);
                    Assert.That(string.IsNullOrEmpty(x.FirstName), Is.False);
                }));
            }
        }

        [Test]
        public void Query_Throws_Exception_When_LazyLoading_After_UnitOfWork_Is_Finished()
        {
            using (var testData = new NHTestDataGenerator(OrdersDomainFactory.OpenSession()))
            {
                testData.Batch(actions =>
                               actions.CreateOrderForCustomer(actions.CreateCustomer()));

                Order order;
                using (new UnitOfWorkScope())
                {
                    var ordersRepository = new NHRepository<Order>();
                    order = (from orders in ordersRepository select orders).FirstOrDefault();
                }

                Assert.That(order, Is.Not.Null);
                Assert.That(NHibernateUtil.IsInitialized(order.Customer), Is.False);
                Assert.Throws<LazyInitializationException>(() => { var firstName = order.Customer.FirstName; });
            }
        }

        [Test]
        public void Query_Using_OrderBy_With_QueryMethod_Returns_Matched_Records_Only()
        {
            using (var testData = new NHTestDataGenerator(OrdersDomainFactory.OpenSession()))
            using (new UnitOfWorkScope())
            {
                testData.Batch(actions =>
                {
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("PA", 2));
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("DE", 5));
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("LA", 3));
                });

                var customersInPA = new Specification<Order>(x => x.Customer.Address.State == "PA");
                var ordersRepository = new NHRepository<Order>();
                var results = from order in ordersRepository.Query(customersInPA)
                              orderby order.OrderDate
                              select order;

                Assert.That(results.Count() > 0);
                Assert.That(results.Count(), Is.EqualTo(2));
            }
        }

        [Test]
        public void Query_Using_QueryMethod_Returns_Matched_Records_Only()
        {
            using (var testData = new NHTestDataGenerator(OrdersDomainFactory.OpenSession()))
            using (new UnitOfWorkScope())
            {
                testData.Batch(actions =>
                {
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("PA", 2));
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("DE", 5));
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("LA", 3));
                });

                var customersInPA = new Specification<Order>(x => x.Customer.Address.State == "DE");

                var ordersRepository = new NHRepository<Order>();
                var results = from order in ordersRepository.Query(customersInPA) select order;

                Assert.That(results.Count(), Is.GreaterThan(0));
                Assert.That(results.Count(), Is.EqualTo(5));
            }
        }

        [Test]
        public void Query_Using_Specifications_With_Closure_Works()
        {
            //This test demonstrates how closures can be used to modify a pre-defined specification using
            //parameters. The specification in this test searches for all customers in the state specified by
            //the queryState local variable. The test then proceeds to build a query using the specification
            //and enumerates over the states array and executes the query by changing the queryState parameter.

            var states = new[] { "PA", "LA" };
            var queryState = string.Empty;

            var spec = new Specification<Order>((order) => order.Customer.Address.State == queryState);
            var repository = new NHRepository<Order>();

            using (var testData = new NHTestDataGenerator(OrdersDomainFactory.OpenSession()))
            using (new UnitOfWorkScope())
            {
                testData.Batch(actions =>
                {
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("PA", 2));
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("DE", 5));
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("LA", 3));
                });

                var query = repository.With(x => x.Customer).Query(spec);
                states.ForEach(testState =>
                {
                    queryState = testState;
                    var results = query.ToArray();
                    results.ForEach(result =>
                                    Assert.That(result.Customer.Address.State, Is.EqualTo(testState)));
                });
            }
        }

        [Test]
        public void Query_With_Incompatible_UnitOfWork_Throws_InvalidOperationException()
        {
            var mockUnitOfWork = MockRepository.GenerateStub<IUnitOfWork>();
            UnitOfWork.Current = mockUnitOfWork;
            Assert.Throws<InvalidOperationException>(() =>
            {
                var customerRepository = new NHRepository<Customer>();
                var results = from customer in customerRepository
                              select customer;
            }
                );
            UnitOfWork.Current = null;
        }

        [Test]
        public void Query_With_No_UnitOfWork_Throws_InvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => { new NHRepository<Customer> { new Customer() }; });
        }

        [Test]
        public void Repository_For_Uses_Registered_Fetching_Strategies()
        {
            IEnumerable<Order> orders;
            using (var testData = new NHTestDataGenerator(OrdersDomainFactory.OpenSession()))
            using (new UnitOfWorkScope())
            {
                testData.Batch(actions =>
                               actions.CreateOrderForProducts(actions.CreateProducts(5)));

                var strategies = new IFetchingStrategy<Order, NHRepositoryTests>[]
                {
                    new OrderOrderItemsStrategy(),
                    new OrderItemsProductStrategy()
                };

                ServiceLocator.Current.Expect(x => x.GetAllInstances<IFetchingStrategy<Order, NHRepositoryTests>>())
                    .Return(strategies);

                orders = new NHRepository<Order>()
                    .For<NHRepositoryTests>()
                    .ToList();
            }
            orders.ForEach(order =>
            {
                Assert.That(NHibernateUtil.IsInitialized(order.Items), Is.True);
                order.Items.ForEach(item =>
                                    Assert.That(NHibernateUtil.IsInitialized(item.Product), Is.True));
            });
        }

        [Test]
        public void Save_Does_Not_Save_New_Customer_When_UnitOfWork_Is_Aborted()
        {
            var rnd = new Random();
            var newAddress = new Address
            {
                StreetAddress1 = "This record was inserted via a test",
                City = "Fictional city",
                State = "LA",
                ZipCode = "12345"
            };

            var newCustomer = new Customer
            {
                FirstName = ("John_" + rnd.Next(60000, 80000)),
                LastName = ("Doe_" + rnd.Next(60000, 80000)),
                Address = newAddress
            };

            using (new UnitOfWorkScope())
            {
                var customerRepository = new NHRepository<Customer>();
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
                var customerRepository = new NHRepository<Customer>();
                var recordCheckResult = (from cust in customerRepository
                                         where cust.FirstName == newCustomer.FirstName &&
                                               cust.LastName == newCustomer.LastName
                                         select cust).FirstOrDefault();
                Assert.That(recordCheckResult, Is.Null);
                scope.Commit();
            }
        }

        [Test]
        public void Save_New_Customer_Saves_Customer_When_UnitOfWork_Is_Committed()
        {
            var newAddress = new Address
            {
                StreetAddress1 = "This record was inserted via a test",
                City = "Fictional city",
                State = "LA",
                ZipCode = "12345"
            };

            var newCustomer = new Customer
            {
                FirstName = ("John_" + DateTime.Now),
                LastName = ("Doe_" + DateTime.Now),
                Address = newAddress
            };

            var queryForCustomer = new Func<NHRepository<Customer>, Customer>
                (
                x => (from cust in x
                      where cust.FirstName == newCustomer.FirstName && cust.LastName == newCustomer.LastName
                      select cust).FirstOrDefault()
                );

            using (var scope = new UnitOfWorkScope())
            {
                var customerRepository = new NHRepository<Customer>();
                var recordCheckResult = queryForCustomer(customerRepository);
                Assert.That(recordCheckResult, Is.Null);

                customerRepository.Add(newCustomer);
                scope.Commit();
            }

            //Starting a completely new unit of work and repository to check for existance.
            using (var scope = new UnitOfWorkScope())
            {
                var customerRepository = new NHRepository<Customer>();
                var recordCheckResult = queryForCustomer(customerRepository);
                Assert.That(recordCheckResult, Is.Not.Null);
                Assert.That(recordCheckResult.FirstName, Is.EqualTo(newCustomer.FirstName));
                Assert.That(recordCheckResult.LastName, Is.EqualTo(newCustomer.LastName));
                customerRepository.Delete(recordCheckResult);
                scope.Commit();
            }
        }

        [Test]
        public void Save_Updates_Existing_Order_Record()
        {
            var updatedDate = DateTime.Now;

            using (var testData = new NHTestDataGenerator(OrdersDomainFactory.OpenSession()))
            {
                testData.Batch(actions =>
                               actions.CreateOrderForCustomer(actions.CreateCustomer()));

                int orderId;
                using (var scope = new UnitOfWorkScope())
                {
                    var orderRepository = new NHRepository<Order>();
                    var order = orderRepository.FirstOrDefault();
                    Assert.That(order, Is.Not.Null);
                    orderId = order.OrderID;
                    order.OrderDate = updatedDate;

                    scope.Commit();
                }

                using (new UnitOfWorkScope())
                {
                    var orderRepository = new NHRepository<Order>();
                    var order = (from o in orderRepository
                                 where o.OrderID == orderId
                                 select o).FirstOrDefault();

                    Assert.That(order, Is.Not.Null);
                    Assert.That(order.OrderDate.Date, Is.EqualTo(updatedDate.Date));
                    Assert.That(order.OrderDate.Hour, Is.EqualTo(updatedDate.Hour));
                    Assert.That(order.OrderDate.Minute, Is.EqualTo(updatedDate.Minute));
                    Assert.That(order.OrderDate.Second, Is.EqualTo(updatedDate.Second));
                }
            }
        }

        [Test]
        public void UnitOfWork_Rolledback_When_Containing_TransactionScope_Is_Rolledback()
        {
            using (var testData = new NHTestDataGenerator(OrdersDomainFactory.OpenSession()))
            {
                testData.Batch(actions =>
                               actions.CreateOrderForCustomer(actions.CreateCustomer()));

                int orderId;
                DateTime oldDate;

                using (var txScope = new TransactionScope(TransactionScopeOption.Required))
                using (var uowScope = new UnitOfWorkScope(System.Data.IsolationLevel.Serializable))
                {
                    var ordersRepository = new NHRepository<Order>();
                    var order = (from o in ordersRepository
                                 select o).First();

                    oldDate = order.OrderDate;
                    order.OrderDate = DateTime.Now;
                    orderId = order.OrderID;
                    uowScope.Commit();
                    //Note: txScope has not been committed
                }

                using (new UnitOfWorkScope())
                {
                    var ordersRepository = new NHRepository<Order>();
                    var order = (from o in ordersRepository
                                 where o.OrderID == orderId
                                 select o).First();

                    Assert.That(order.OrderDate, Is.EqualTo(oldDate));
                }
            }
        }

        [Test]
        public void When_Calling_CalculateTotal_On_Order_Returns_Valid_When_Under_UnitOfWork()
        {
            using (var testData = new NHTestDataGenerator(OrdersDomainFactory.OpenSession()))
            using (new UnitOfWorkScope())
            {
                testData.Batch(actions =>
                               actions.CreateOrderForProducts(actions.CreateProducts(5)));

                var oredersRepository = new NHRepository<Order>();
                var order = (from o in oredersRepository
                             select o).FirstOrDefault();

                Assert.That(order.CalculateTotal(), Is.GreaterThan(0));
            }
        }

        [Test]
        public void When_Calling_CalculateTotal_On_Order_Returns_Valid_With_No_UnitOfWork_Throws()
        {
            Order order;
            using (var testData = new NHTestDataGenerator(OrdersDomainFactory.OpenSession()))
            using (new UnitOfWorkScope())
            {
                testData.Batch(actions =>
                               actions.CreateOrderForProducts(actions.CreateProducts(5)));

                var oredersRepository = new NHRepository<Order>();
                order = (from o in oredersRepository
                         select o).FirstOrDefault();
            }
            Assert.Throws<LazyInitializationException>(() => order.CalculateTotal());
        }

        [Test]
        public void When_No_FetchingStrategy_Registered_For_Makes_No_Changes()
        {
            Order order;
            using (var testData = new NHTestDataGenerator(OrdersDomainFactory.OpenSession()))
            using (new UnitOfWorkScope())
            {
                testData.Batch(actions =>
                               actions.CreateOrderForCustomer(actions.CreateCustomer()));

                var oredersRepository = new NHRepository<Order>().For<NHRepositoryTests>();
                order = (from o in oredersRepository
                         select o).FirstOrDefault();
            }
            Assert.That(NHibernateUtil.IsInitialized(order.Customer), Is.False);
        }

        [Test]
        public void can_query_multiple_data_sources()
        {
            using (var ordersDomainTestData = new NHTestDataGenerator(OrdersDomainFactory.OpenSession()))
            using (var hrDomainTestData = new NHTestDataGenerator(HRDomainFactory.OpenSession()))
            {
                var customerId = 0;
                var salesPersonId = 0;
                ordersDomainTestData.Batch(action => customerId = action.CreateCustomer().CustomerID);
                hrDomainTestData.Batch(action => salesPersonId = action.CreateSalesPerson().Id);

                using (var scope = new UnitOfWorkScope())
                {
                    var customerRepository = new NHRepository<Customer>();
                    var salesPersonRepository = new NHRepository<SalesPerson>();
                    var customer = customerRepository.Where(x => x.CustomerID == customerId).FirstOrDefault();
                    var salesPerson = salesPersonRepository.Where(x => x.Id == salesPersonId).FirstOrDefault();

                    Assert.That(customer, Is.Not.Null);
                    Assert.That(salesPerson, Is.Not.Null);
                    scope.Commit();
                }
            }
        }
    }
}