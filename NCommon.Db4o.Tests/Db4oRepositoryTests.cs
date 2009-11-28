using System;
using System.IO;
using System.Linq;
using System.Transactions;
using Db4objects.Db4o;
using Microsoft.Practices.ServiceLocation;
using NCommon.Data;
using NCommon.Db4o.Tests.Domain;
using NCommon.Extensions;
using NCommon.Specifications;
using NUnit.Framework;
using Rhino.Mocks;
using IsolationLevel=System.Data.IsolationLevel;

namespace NCommon.Db4o.Tests
{
    [TestFixture]
    public class Db4oRepositoryTests
    {
        IServiceLocator _mockLocator;
        IObjectServer _db4oServer;
        readonly string _databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testdatabase.db4o");

        [TestFixtureSetUp]
        public void Setup()
        {
            if (File.Exists(_databasePath))
                File.Delete(_databasePath);

            //Configure Db4o to cascade updates on Order and Customer entities. (Since they are our Aggregate Roots)
            var configuration = Db4oFactory.Configure();
            configuration.ObjectClass(typeof (Order)).CascadeOnUpdate(true);
            configuration.ObjectClass(typeof (Customer)).CascadeOnDelete(true);

            _db4oServer = Db4oFactory.OpenServer(_databasePath, 0);
            Db4oUnitOfWorkFactory.SetContainerProvider(() => _db4oServer.OpenClient());
            _mockLocator = MockRepository.GenerateMock<IServiceLocator>();
            _mockLocator.Expect(x => x.GetInstance<IUnitOfWorkFactory>())
                .Return(new Db4oUnitOfWorkFactory())
                .Repeat.Any();
            ServiceLocator.SetLocatorProvider(() => _mockLocator);
        }

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
            var queryForCustomer = new Func<Db4oRepository<Customer>, Customer>
                (x => (from cust in x
                       where cust.FirstName == newCustomer.FirstName && cust.LastName == newCustomer.LastName
                       select cust).FirstOrDefault()
                );

            using (var scope = new UnitOfWorkScope())
            {
                var customerRepository = new Db4oRepository<Customer>();
                var recordCheckResult = queryForCustomer(customerRepository);
                Assert.That(recordCheckResult, Is.Null);

                customerRepository.Add(newCustomer);
                scope.Commit();
            }

            //Retrieve the record for deletion.
            using (var scope = new UnitOfWorkScope())
            {
                var customerRepository = new Db4oRepository<Customer>();
                var customerToDelete = queryForCustomer(customerRepository);
                Assert.That(customerToDelete, Is.Not.Null);
                customerRepository.Delete(customerToDelete);
                scope.Commit();
            }

            //Ensure customer record is deleted.
            using (new UnitOfWorkScope())
            {
                var customerRepository = new Db4oRepository<Customer>();
                var recordCheckResult = queryForCustomer(customerRepository);
                Assert.That(recordCheckResult, Is.Null);
            }
        }

        [Test]
        public void Nested_UnitOfWork_With_Different_Transaction_Compatibility_Works()
        {
            var changedShipDate = DateTime.Now.AddDays(1);
            var changedOrderDate = DateTime.Now.AddDays(2);

            using (var testData = new Db4oTestDataGenerator(_db4oServer.OpenClient()))
            {
                testData.Batch(actions =>
                               actions.CreateOrderForCustomer(actions.CreateCustomer()));

                int orderId;
                using (new UnitOfWorkScope())
                {
                    var ordersRepository = new Db4oRepository<Order>();
                    orderId = ordersRepository.Select(x => x.OrderID).First();
                }

                Assert.NotNull(orderId);
                using (new UnitOfWorkScope())
                {
                    var outerRepository = new Db4oRepository<Order>();
                    var outerOrder = outerRepository.Where(x => x.OrderID == orderId).First();
                    outerOrder.OrderDate = changedOrderDate;

                    using (var innerScope = new UnitOfWorkScope(UnitOfWorkScopeTransactionOptions.CreateNew))
                    {
                        var innerRepository = new Db4oRepository<Order>();
                        var innerOrder = innerRepository.Where(x => x.OrderID == orderId).First();
                        innerOrder.ShipDate = changedShipDate;
                        innerScope.Commit();
                    }
                }

                using (new UnitOfWorkScope())
                {
                    var ordersRepository = new Db4oRepository<Order>();
                    var order = ordersRepository.First();
                    Assert.That(order.OrderDate, Is.Not.EqualTo(changedOrderDate));
                    Assert.That(order.ShipDate, Is.Not.EqualTo(changedShipDate));
                }
            }
        }

        [Test]
        public void Query_Allows_Projection_Using_Select_Projection()
        {
            using (var testData = new Db4oTestDataGenerator(_db4oServer.OpenClient()))
            using (new UnitOfWorkScope())
            {
                testData.Batch(actions =>
                               actions.CreateOrderForCustomer(actions.CreateCustomer()));

                var ordersRepository = new Db4oRepository<Order>();
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
        public void Query_Using_OrderBy_With_QueryMethod_Returns_Matched_Records_Only()
        {
            using (var testData = new Db4oTestDataGenerator(_db4oServer.OpenClient()))
            using (new UnitOfWorkScope())
            {
                testData.Batch(actions =>
                {
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("PA", 2));
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("DE", 5));
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("LA", 3));
                });

                var customersInPA = new Specification<Order>(x => x.Customer.Address.State == "PA");
                var ordersRepository = new Db4oRepository<Order>();
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
            using (var testData = new Db4oTestDataGenerator(_db4oServer.OpenClient()))
            using (new UnitOfWorkScope())
            {
                testData.Batch(actions =>
                {
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("PA", 2));
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("DE", 5));
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("LA", 3));
                });

                var customersInPA = new Specification<Order>(x => x.Customer.Address.State == "DE");

                var ordersRepository = new Db4oRepository<Order>();
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
            var repository = new Db4oRepository<Order>();

            using (var testData = new Db4oTestDataGenerator(_db4oServer.OpenClient()))
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
        public void Query_With_No_UnitOfWork_Throws_InvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => { new Db4oRepository<Customer> { new Customer() }; });
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
                var customerRepository = new Db4oRepository<Customer>();
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
                var customerRepository = new Db4oRepository<Customer>();
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

            var queryForCustomer = new Func<Db4oRepository<Customer>, Customer>
                (
                x => (from cust in x
                      where cust.FirstName == newCustomer.FirstName && cust.LastName == newCustomer.LastName
                      select cust).FirstOrDefault()
                );

            using (var scope = new UnitOfWorkScope())
            {
                var customerRepository = new Db4oRepository<Customer>();
                var recordCheckResult = queryForCustomer(customerRepository);
                Assert.That(recordCheckResult, Is.Null);

                customerRepository.Add(newCustomer);
                scope.Commit();
            }

            //Starting a completely new unit of work and repository to check for existance.
            using (var scope = new UnitOfWorkScope())
            {
                var customerRepository = new Db4oRepository<Customer>();
                var recordCheckResult = queryForCustomer(customerRepository);
                Assert.That(recordCheckResult, Is.Not.Null);
                Assert.That(recordCheckResult.FirstName, Is.EqualTo(newCustomer.FirstName));
                Assert.That(recordCheckResult.LastName, Is.EqualTo(newCustomer.LastName));
                scope.Commit();
            }
        }

        [Test]
        public void Save_Updates_Existing_Order_Record()
        {
            var updatedDate = DateTime.Now;
            using (var testData = new Db4oTestDataGenerator(_db4oServer.OpenClient()))
            {
                testData.Batch(actions =>
                               actions.CreateOrderForCustomer(actions.CreateCustomer()));

                int orderId;
                using (var scope = new UnitOfWorkScope())
                {
                    var orderRepository = new Db4oRepository<Order>();
                    var order = orderRepository.FirstOrDefault();
                    Assert.That(order, Is.Not.Null);
                    orderId = order.OrderID;
                    order.OrderDate = updatedDate;
                    orderRepository.Save(order); //Db4o does not do change tracking!
                    scope.Commit();
                }

                using (new UnitOfWorkScope())
                {
                    var orderRepository = new Db4oRepository<Order>();
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
            using (var testData = new Db4oTestDataGenerator(_db4oServer.OpenClient()))
            {
                testData.Batch(actions =>
                               actions.CreateOrderForCustomer(actions.CreateCustomer()));

                int orderId;
                DateTime oldDate;

                using (var txScope = new TransactionScope(TransactionScopeOption.Required))
                using (var uowScope = new UnitOfWorkScope(IsolationLevel.Serializable))
                {
                    var ordersRepository = new Db4oRepository<Order>();
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
                    var ordersRepository = new Db4oRepository<Order>();
                    var order = (from o in ordersRepository
                                 where o.OrderID == orderId
                                 select o).First();

                    Assert.That(order.OrderDate, Is.EqualTo(oldDate));
                }
            }
        }
    }
}