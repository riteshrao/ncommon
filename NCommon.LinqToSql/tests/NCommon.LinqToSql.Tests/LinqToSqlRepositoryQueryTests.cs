using System;
using System.Data.Linq;
using System.Linq;
using NCommon.LinqToSql.Tests.HRDomain;
using NCommon.LinqToSql.Tests.OrdersDomain;
using NCommon.DataServices.Transactions;
using NCommon.Extensions;

using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.LinqToSql.Tests
{
    [TestFixture]
    public class LinqToSqlRepositoryQueryTests : LinqToSqlRepositoryTestBase
    {
        [Test]
        public void can_perform_simple_query()
        {
            using (var testData = new LinqToSqlTestData(OrdersContextProvider()))
            {
                Customer customer = null;
                testData.Batch(x => customer = x.CreateCustomer());

                using (var scope = new UnitOfWorkScope())
                {
                    var savedCustomer = new LinqToSqlRepository<Customer>()
                        .Where(x => x.CustomerID == customer.CustomerID)
                        .First();

                    Assert.That(savedCustomer, Is.Not.Null);
                    Assert.That(savedCustomer.CustomerID, Is.EqualTo(customer.CustomerID));
                    scope.Commit();
                }
            }
        }

        [Test]
        public void can_save()
        {
            var customer = new Customer
            {
                FirstName = "Jane",
                LastName = "Doe",
                StreetAddress1 = "123 Main St",
                City = "Sunset City",
                State = "LA",
                ZipCode = "12345"
            };

            using (var scope = new UnitOfWorkScope())
            {
                var repository = new LinqToSqlRepository<Customer>();
                repository.Add(customer);
                scope.Commit();
            }
            Assert.That(customer.CustomerID, Is.GreaterThan(0));
            using (var testData = new LinqToSqlTestData(OrdersContextProvider()))
            {
                Customer savedCustomer = null;
                testData.Batch(action => savedCustomer = action.GetCustomerById(customer.CustomerID));
                Assert.That(savedCustomer, Is.Not.Null);
                Assert.That(savedCustomer.CustomerID, Is.EqualTo(customer.CustomerID));
            }
        }

        [Test]
        public void can_modify()
        {
            using (var testData = new LinqToSqlTestData(OrdersContextProvider()))
            {
                Customer customer = null;
                testData.Batch(x => customer = x.CreateCustomer());

                using (var scope = new UnitOfWorkScope())
                {
                    var savedCustomer = new LinqToSqlRepository<Customer>()
                        .Where(x => x.CustomerID == customer.CustomerID)
                        .First();

                    savedCustomer.LastName = "Changed";
                    scope.Commit();
                }

                testData.Context<OrdersDataDataContext>().Refresh(RefreshMode.OverwriteCurrentValues, customer);
                Assert.That(customer.LastName, Is.EqualTo("Changed"));
            }
        }

        [Test]
        public void can_delete()
        {
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
            };
            using (var scope = new UnitOfWorkScope())
            {
                new LinqToSqlRepository<Customer>().Add(customer);
                scope.Commit();
            }
            Assert.That(customer.CustomerID, Is.GreaterThan(0));
            using (var scope = new UnitOfWorkScope())
            {
                var repository = new LinqToSqlRepository<Customer>();
                var savedCustomer = repository.Where(x => x.CustomerID == customer.CustomerID).First();
                repository.Delete(savedCustomer);
                scope.Commit();
            }

            //Making sure customer is deleted
            using (var testData = new LinqToSqlTestData(OrdersContextProvider()))
            {
                Customer savedCustomer = null;
                testData.Batch(x => savedCustomer = x.GetCustomerById(customer.CustomerID));
                Assert.That(savedCustomer, Is.Null);
            }
        }

        [Test]
        public void can_attach()
        {
            var customer = new Customer
            {
                FirstName = "Jane",
                LastName = "Doe"
            };

            var context = OrdersContextProvider() as OrdersDataDataContext;
            context.Customers.InsertOnSubmit(customer);
            context.SubmitChanges();
            context.Dispose(); //Auto detach

            using (var scope = new UnitOfWorkScope())
            {
                var repository = new LinqToSqlRepository<Customer>();
                repository.Attach(customer);
                customer.LastName = "Changed";
                scope.Commit(); //Should change since the customer was attached to repository.
            }

            using (var testData = new LinqToSqlTestData(OrdersContextProvider()))
            {
                Customer savedCustomer = null;
                testData.Batch(x => savedCustomer = x.GetCustomerById(customer.CustomerID));
                Assert.That(savedCustomer, Is.Not.Null);
                Assert.That(savedCustomer.LastName, Is.EqualTo("Changed"));
            }
        }

        [Test]
        public void can_query_using_specification()
        {
            using (var testData = new LinqToSqlTestData(OrdersContextProvider()))
            {
                testData.Batch(actions =>
                {
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("PA", 2));
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("DE", 5));
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("LA", 3));
                });

                using (new UnitOfWorkScope())
                {


                    var customersInPA = new Specification<Order>(x => x.Customer.State == "DE");

                    var ordersRepository = new LinqToSqlRepository<Order>();
                    var results = from order in ordersRepository.Query(customersInPA) select order;

                    Assert.That(results.Count(), Is.GreaterThan(0));
                    Assert.That(results.Count(), Is.EqualTo(5));
                }
            }
        }

        [Test]
        public void can_lazyload()
        {
            using (var testData = new LinqToSqlTestData(OrdersContextProvider()))
            {
                Customer customer = null;
                testData.Batch(x =>
                {
                    customer = x.CreateCustomer();
                    x.CreateOrderForCustomer(customer);
                });

                using (var scope = new UnitOfWorkScope())
                {
                    var savedCustomer = new LinqToSqlRepository<Customer>()
                        .Where(x => x.CustomerID == customer.CustomerID)
                        .First();

                    Assert.That(savedCustomer, Is.Not.Null);
                    Assert.That(savedCustomer.Orders, Is.Not.Null);
                    Assert.That(savedCustomer.Orders.Count, Is.GreaterThan(0));
                    scope.Commit();
                }
            }
        }

        [Test]
        public void lazyloading_when_outside_scope_throws()
        {
            using (var testData = new LinqToSqlTestData(OrdersContextProvider()))
            {
                Order order = null;
                testData.Batch(x => order = x.CreateOrderForCustomer(x.CreateCustomer()));

                Order savedOrder = null;
                using (var scope = new UnitOfWorkScope())
                {
                    savedOrder = new LinqToSqlRepository<Order>()
                        .Where(x => x.OrderID == order.OrderID)
                        .First();
                    scope.Commit();
                }
                Assert.That(savedOrder, Is.Not.Null);
                Assert.Throws<ObjectDisposedException>(() => { var customer = savedOrder.Customer.FirstName; });
            }
        }

        [Test]
        public void can_query_multiple_databases()
        {
            using (var ordersTestData = new LinqToSqlTestData(OrdersContextProvider()))
            using (var hrTestData = new LinqToSqlTestData(HRContextProvider()))
            {
                Customer customer = null;
                SalesPerson salesPerson = null;
                ordersTestData.Batch(x => customer = x.CreateCustomer());
                hrTestData.Batch(x => salesPerson = x.CreateSalesPerson());

                //NOTE: This will enlist a Distributed DTC tx.
                using (var scope = new UnitOfWorkScope())
                {
                    var savedCustomer = new LinqToSqlRepository<Customer>()
                        .Where(x => x.CustomerID == customer.CustomerID)
                        .First();

                    var savedPerson = new LinqToSqlRepository<SalesPerson>()
                        .Where(x => x.Id == salesPerson.Id)
                        .First();

                    Assert.That(savedCustomer, Is.Not.Null);
                    Assert.That(savedPerson, Is.Not.Null);
                    scope.Commit();
                }
            }
        }
    }
}