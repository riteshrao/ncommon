using System;
using System.Linq;
using NCommon.Data.NHibernate.Tests.HRDomain.Domain;
using NCommon.Data.NHibernate.Tests.OrdersDomain;
using NUnit.Framework;

namespace NCommon.Data.NHibernate.Tests
{
    [TestFixture]
    public class NHRepositoryTransactionTests : NHRepositoryTestBase
    {
        [Test]
        public void can_commit()
        {
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe"
            };
            using (var scope = new UnitOfWorkScope())
            {
                new NHRepository<Customer>()
                    .Add(customer);
                scope.Commit();
            }

            using (var testData = new NHTestData(OrdersDomainFactory.OpenSession()))
            {
                Customer savedCustomer = null;
                testData.Batch(action => savedCustomer = action.GetCustomerById(customer.CustomerID));

                Assert.That(savedCustomer, Is.Not.Null);
                Assert.That(savedCustomer.CustomerID, Is.EqualTo(customer.CustomerID));
            }

        }

        [Test]
        public void can_rollback()
        {
            using (var testData = new NHTestData(OrdersDomainFactory.OpenSession()))
            {
                Customer customer = null;
                testData.Batch(action => customer = action.CreateCustomer());

                using (new UnitOfWorkScope())
                {
                    var savedCustomer = new NHRepository<Customer>()
                        .Where(x => x.CustomerID == customer.CustomerID)
                        .First();
                    savedCustomer.LastName = "Changed";
                } //Dispose here as scope is not comitted.

                testData.Session.Refresh(customer);
                Assert.That(customer.LastName, Is.Not.EqualTo("Changed"));
            }
        }

        [Test]
        public void nested_commit_works()
        {
            var customer = new Customer { FirstName = "Joe", LastName = "Data" };
            var order = new Order {OrderDate = DateTime.Now, ShipDate = DateTime.Now};
            using (var scope = new UnitOfWorkScope())
            {
                new NHRepository<Customer>().Add(customer);
                using (var scope2 = new UnitOfWorkScope())
                {
                    new NHRepository<Order>().Add(order);
                    scope2.Commit();
                }
                scope.Commit();
            }

            using (var testData = new NHTestData(OrdersDomainFactory.OpenSession()))
            {
                Customer savedCustomer = null;
                Order savedOrder = null;
                testData.Batch(actions =>
                {
                    savedCustomer = actions.GetCustomerById(customer.CustomerID);
                    savedOrder = actions.GetOrderById(order.OrderID);
                });

                Assert.That(savedCustomer, Is.Not.Null);
                Assert.That(savedCustomer.CustomerID, Is.EqualTo(customer.CustomerID));
                Assert.That(savedOrder, Is.Not.Null);
                Assert.That(savedOrder.OrderID, Is.EqualTo(order.OrderID));
            }
        }

        [Test]
        public void nested_commit_with_seperate_transaction_commits_when_wrapping_scope_rollsback()
        {
            var customer = new Customer { FirstName = "Joe", LastName = "Data" };
            var order = new Order { OrderDate = DateTime.Now, ShipDate = DateTime.Now };
            using (var scope = new UnitOfWorkScope())
            {
                new NHRepository<Customer>().Add(customer);
                using (var scope2 = new UnitOfWorkScope(TransactionMode.New))
                {
                    new NHRepository<Order>().Add(order);
                    scope2.Commit();
                }
            } //Rollback

            using (var testData = new NHTestData(OrdersDomainFactory.OpenSession()))
            {
                Customer savedCustomer = null;
                Order savedOrder = null;
                testData.Batch(actions =>
                {
                    savedCustomer = actions.GetCustomerById(customer.CustomerID);
                    savedOrder = actions.GetOrderById(order.OrderID);
                });

                Assert.That(savedCustomer, Is.Null);
                Assert.That(savedOrder, Is.Not.Null);
                Assert.That(savedOrder.OrderID, Is.EqualTo(order.OrderID));
            }
        }

        [Test]
        public void nested_rollback_works()
        {
            var customer = new Customer { FirstName = "Joe", LastName = "Data" };
            var order = new Order { OrderDate = DateTime.Now, ShipDate = DateTime.Now };
            using (var scope = new UnitOfWorkScope())
            {
                new NHRepository<Customer>().Add(customer);
                using (var scope2 = new UnitOfWorkScope())
                {
                    new NHRepository<Order>().Add(order);
                    scope2.Commit();
                } 
            } //Rollback.

            using (var testData = new NHTestData(OrdersDomainFactory.OpenSession()))
            {
                Customer savedCustomer = null;
                Order savedOrder = null;
                testData.Batch(actions =>
                {
                    savedCustomer = actions.GetCustomerById(customer.CustomerID);
                    savedOrder = actions.GetOrderById(order.OrderID);
                });

                Assert.That(savedCustomer, Is.Null);
                Assert.That(savedOrder, Is.Null);
            }
        }

        [Test]
        public void commit_throws_when_child_scope_rollsback()
        {
            var customer = new Customer { FirstName = "Joe", LastName = "Data" };
            var order = new Order { OrderDate = DateTime.Now, ShipDate = DateTime.Now };
            using (var scope = new UnitOfWorkScope())
            {
                new NHRepository<Customer>().Add(customer);
                using (var scope2 = new UnitOfWorkScope())
                {
                    new NHRepository<Order>().Add(order);
                } //child scope rollback.
                Assert.Throws<InvalidOperationException>(scope.Commit);
            } 
        }

        [Test]
        public void can_commit_multiple_db_operations()
        {
            var customer = new Customer {FirstName = "John", LastName = "Doe"};
            var salesPerson = new SalesPerson {FirstName = "Jane", LastName = "Doe", SalesQuota = 2000};

            using (var scope = new UnitOfWorkScope())
            {
                new NHRepository<Customer>().Add(customer);
                new NHRepository<SalesPerson>().Add(salesPerson);
                scope.Commit();
            }

            using (var ordersTestData = new NHTestData(OrdersDomainFactory.OpenSession()))
            using (var hrTestData = new NHTestData(HRDomainFactory.OpenSession()))
            {
                Customer savedCustomer = null;
                SalesPerson savedSalesPerson = null;
                ordersTestData.Batch(action => savedCustomer = action.GetCustomerById(customer.CustomerID));
                hrTestData.Batch(action => savedSalesPerson = action.GetSalesPersonById(salesPerson.Id));

                Assert.That(savedCustomer, Is.Not.Null);
                Assert.That(savedSalesPerson, Is.Not.Null);
                Assert.That(savedCustomer.CustomerID, Is.EqualTo(customer.CustomerID));
                Assert.That(savedSalesPerson.Id, Is.EqualTo(salesPerson.Id));
            }
        }

        [Test]
        public void can_rollback_multipe_db_operations()
        {
            var customer = new Customer { FirstName = "John", LastName = "Doe" };
            var salesPerson = new SalesPerson { FirstName = "Jane", LastName = "Doe", SalesQuota = 2000 };

            using (var scope = new UnitOfWorkScope())
            {
                new NHRepository<Customer>().Add(customer);
                new NHRepository<SalesPerson>().Add(salesPerson);
            }// Rolllback

            using (var ordersTestData = new NHTestData(OrdersDomainFactory.OpenSession()))
            using (var hrTestData = new NHTestData(HRDomainFactory.OpenSession()))
            {
                Customer savedCustomer = null;
                SalesPerson savedSalesPerson = null;
                ordersTestData.Batch(action => savedCustomer = action.GetCustomerById(customer.CustomerID));
                hrTestData.Batch(action => savedSalesPerson = action.GetSalesPersonById(salesPerson.Id));

                Assert.That(savedCustomer, Is.Null);
                Assert.That(savedSalesPerson, Is.Null);
            }
        }

        [Test]
        public void rollback_does_not_rollback_supressed_scope()
        {
            var customer = new Customer { FirstName = "Joe", LastName = "Data" };
            var order = new Order { OrderDate = DateTime.Now, ShipDate = DateTime.Now };
            using (var scope = new UnitOfWorkScope())
            {
                new NHRepository<Customer>().Add(customer);
                using (var scope2 = new UnitOfWorkScope(TransactionMode.Supress))
                {
                    new NHRepository<Order>().Add(order);
                    scope2.Commit();
                }
            } //Rollback.

            using (var testData = new NHTestData(OrdersDomainFactory.OpenSession()))
            {
                Customer savedCustomer = null;
                Order savedOrder = null;
                testData.Batch(actions =>
                {
                    savedCustomer = actions.GetCustomerById(customer.CustomerID);
                    savedOrder = actions.GetOrderById(order.OrderID);
                });

                Assert.That(savedCustomer, Is.Null);
                Assert.That(savedOrder, Is.Not.Null);
            }
        }
    }
}