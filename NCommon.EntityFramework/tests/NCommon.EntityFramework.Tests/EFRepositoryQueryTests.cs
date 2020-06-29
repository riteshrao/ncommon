using System;
using System.Data.Objects;
using System.Linq;
using NCommon.Data.EntityFramework.Tests.HRDomain;
using NCommon.Data.EntityFramework.Tests.OrdersDomain;

using NUnit.Framework;

namespace NCommon.Data.EntityFramework.Tests
{
    [TestFixture]
    public class EFRepositoryQueryTests : EFRepositoryTestBase
    {
        [Test]
        public void Can_perform_simple_query()
        {
            using (var testData = new EFTestData(OrdersContextProvider()))
            {
                Customer customer = null;
                testData.Batch(x => customer = x.CreateCustomer());

                using (var scope = new UnitOfWorkScope())
                {
                    var savedCustomer = new EFRepository<Customer>()
                        .Where(x => x.CustomerID == customer.CustomerID)
                        .First();

                    Assert.That(savedCustomer, Is.Not.Null);
                    Assert.That(savedCustomer.CustomerID, Is.EqualTo(customer.CustomerID));
                    scope.Commit();
                }
            }
        }

        [Test]
        public void Can_save()
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
                var repository = new EFRepository<Customer>();
                repository.Add(customer);
                scope.Commit();
            }
            Assert.That(customer.CustomerID, Is.GreaterThan(0));
            using (var testData = new EFTestData(OrdersContextProvider()))
            {
                Customer savedCustomer = null;
                testData.Batch(action => savedCustomer = action.GetCustomerById(customer.CustomerID));
                Assert.That(savedCustomer, Is.Not.Null);
                Assert.That(savedCustomer.CustomerID, Is.EqualTo(customer.CustomerID));
            }
        }

        [Test]
        public void Can_modify()
        {
            using (var testData = new EFTestData(OrdersContextProvider()))
            {
                Customer customer = null;
                testData.Batch(x => customer = x.CreateCustomer());

                using (var scope = new UnitOfWorkScope())
                {
                    var savedCustomer = new EFRepository<Customer>()
                        .Where(x => x.CustomerID == customer.CustomerID)
                        .First();

                    savedCustomer.LastName = "Changed";
                    scope.Commit();
                }

                testData.Context<OrderEntities>().Refresh(RefreshMode.StoreWins, customer);
                Assert.That(customer.LastName, Is.EqualTo("Changed"));
            }
        }

        [Test]
        public void Can_delete()
        {
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
            };
            using (var scope = new UnitOfWorkScope())
            {
                new EFRepository<Customer>().Add(customer);
                scope.Commit();
            }
            Assert.That(customer.CustomerID, Is.GreaterThan(0));
            using (var scope = new UnitOfWorkScope())
            {
                var repository = new EFRepository<Customer>();
                var savedCustomer = repository.Where(x => x.CustomerID == customer.CustomerID).First();
                repository.Delete(savedCustomer);
                scope.Commit();
            }

            //Making sure customer is deleted
            using (var testData = new EFTestData(OrdersContextProvider()))
            {
                Customer savedCustomer = null;
                testData.Batch(x => savedCustomer = x.GetCustomerById(customer.CustomerID));
                Assert.That(savedCustomer, Is.Null);
            }
        }

        [Test]
        public void Can_attach()
        {
            var customer = new Customer
            {
                FirstName = "Jane",
                LastName = "Doe"
            };

            var context = (OrderEntities) OrdersContextProvider();
            context.AddToCustomers(customer);
#if EF_1_0
            context.SaveChanges(true);
#else
            context.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
#endif
            context.Detach(customer);
            context.Dispose(); //Auto detach

            using (var scope = new UnitOfWorkScope())
            {
                var repository = new EFRepository<Customer>();
                repository.Attach(customer);
                customer.LastName = "Changed";
                scope.Commit(); //Should change since the customer was attached to repository.
            }

            using (var testData = new EFTestData(OrdersContextProvider()))
            {
                Customer savedCustomer = null;
                testData.Batch(x => savedCustomer = x.GetCustomerById(customer.CustomerID));
                Assert.That(savedCustomer, Is.Not.Null);
                Assert.That(savedCustomer.LastName, Is.EqualTo("Changed"));
            }
        }

        [Test]
        public void Can_attach_modified_entity()
        {
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe"
            };

            var context = (OrderEntities) OrdersContextProvider();
            context.AddToCustomers(customer);
#if EF_1_0
            context.SaveChanges(true);
#else
            context.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
#endif
            context.Detach(customer);
            context.Dispose();

            using (var scope = new UnitOfWorkScope())
            {
                customer.LastName = "Changed";
                var repository = new EFRepository<Customer>();
                repository.Attach(customer);
                scope.Commit();
            }

            using (var testData = new EFTestData(OrdersContextProvider()))
            {
                Customer savedCustomer = null;
                testData.Batch(x => savedCustomer = x.GetCustomerById(customer.CustomerID));
                Assert.That(savedCustomer, Is.Not.Null);
                Assert.That(savedCustomer.LastName, Is.EqualTo("Changed"));
            }
        }

        [Test]
        public void Can_query_using_specification()
        {
            using (var testData = new EFTestData(OrdersContextProvider()))
            {
                testData.Batch(actions =>
                {
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("PA", 2));
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("DE", 5));
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("LA", 3));
                });

                using (new UnitOfWorkScope())
                {


                    var customersInPa = new Specification<Order>(x => x.Customer.State == "DE");

                    var ordersRepository = new EFRepository<Order>();
                    var results = from order in ordersRepository.Query(customersInPa) select order;

                    Assert.That(results.Count(), Is.GreaterThan(0));
                    Assert.That(results.Count(), Is.EqualTo(5));
                }
            }
        }

        [Test]
        public void Can_lazyload()
        {
            using (var testData = new EFTestData(OrdersContextProvider()))
            {
                Customer customer = null;
                testData.Batch(x =>
                {
                    customer = x.CreateCustomer();
                    x.CreateOrderForCustomer(customer);
                });

                using (var scope = new UnitOfWorkScope())
                {
                    var savedCustomer = new EFRepository<Customer>()
                        .Where(x => x.CustomerID == customer.CustomerID)
                        .First();

                    Assert.That(savedCustomer, Is.Not.Null);
                    Assert.That(savedCustomer.Orders, Is.Not.Null);
                    Assert.DoesNotThrow(savedCustomer.Orders.Load);
                    Assert.That(savedCustomer.Orders.Count, Is.GreaterThan(0));
                    scope.Commit();
                }
            }
        }

        [Test]
        public void Lazyloading_when_outside_scope_throws()
        {
            using (var testData = new EFTestData(OrdersContextProvider()))
            {
                Order order = null;
                testData.Batch(x => order = x.CreateOrderForCustomer(x.CreateCustomer()));

                Order savedOrder;
                using (var scope = new UnitOfWorkScope())
                {
                    savedOrder = new EFRepository<Order>()
                        .Where(x => x.OrderID == order.OrderID)
                        .First();
                    scope.Commit();
                }
                Assert.That(savedOrder, Is.Not.Null);
                Assert.Throws<ObjectDisposedException>(() => { var customer = savedOrder.Customer; });
            }
        }

        [Test]
        public void Can_query_multiple_databases()
        {
            using (var ordersTestData = new EFTestData(OrdersContextProvider()))
            using (var hrTestData = new EFTestData(HRContextProvider()))
            {
                Customer customer = null;
                SalesPerson salesPerson = null;
                ordersTestData.Batch(x => customer = x.CreateCustomer());
                hrTestData.Batch(x => salesPerson = x.CreateSalesPerson());

                //Suprisingly this does not enlist in a DTC transaction. EF is able to re-use the same connection
                //since both ObjectContext connect to the same database instance.
                using (var scope = new UnitOfWorkScope())
                {
                    var savedCustomer = new EFRepository<Customer>()
                        .Where(x => x.CustomerID == customer.CustomerID)
                        .First();

                    var savedPerson = new EFRepository<SalesPerson>()
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