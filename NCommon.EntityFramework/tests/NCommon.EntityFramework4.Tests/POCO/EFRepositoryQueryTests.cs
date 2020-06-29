using System;
using System.Configuration;
using System.Linq;

using NCommon.DataServices.Transactions;
using NCommon.Data.EntityFramework;
using NCommon.EntityFramework4.Tests.Models;
using NCommon.Extensions;

using NCommon.StateStorage;
using NCommon.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.EntityFramework4.Tests.POCO
{
    [TestFixture]
    public class EFRepositoryQueryTests : EFRepositoryQueryTestsBase
    {
        [Test]
        public void Can_perform_simple_query()
        {
            var testData = new EFTestData(Context);
            Customer customer = null;
            testData.Batch(x => customer = x.CreateCustomer());
            using (var scope = new UnitOfWorkScope())
            {
                var savedCustomer = new EFRepository<Customer>()
                    .First(x => x.CustomerID == customer.CustomerID);
                Assert.That(savedCustomer, Is.Not.Null);
                scope.Commit();
            }
        }

        [Test]
        public void Can_save()
        {
            int customerId;
            using (var scope = new UnitOfWorkScope())
            {
                var customer = new Customer
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    StreetAddress1 = "123 Main St.",
                };
                new EFRepository<Customer>().Add(customer);
                scope.Commit();
                customerId = customer.CustomerID;
            }
            var savedCustomer = new EFTestData(Context)
                .Get<Customer>(x => x.CustomerID == customerId);
            Assert.That(savedCustomer, Is.Not.Null);
        }

        [Test]
        public void Can_modify()
        {
            Customer customer = null;
            var testData = new EFTestData(Context);
            testData.Batch(x => customer = x.CreateCustomer());
            using (var scope = new UnitOfWorkScope())
            {
                var savedCustomer = new EFRepository<Customer>()
                    .First(x => x.CustomerID == customer.CustomerID);
                savedCustomer.FirstName = "Changed";
                scope.Commit();
            }

            testData.Refresh(customer);
            Assert.That(customer.FirstName, Is.EqualTo("Changed"));
        }

        [Test]
        public void Can_delete()
        {
            Customer customer = null;
            var testData = new EFTestData(Context);
            testData.Batch(x => customer = x.CreateCustomer());
            using (var scope = new UnitOfWorkScope())
            {
                var repository = new EFRepository<Customer>();
                var savedCustomer = repository.First(x => x.CustomerID == customer.CustomerID);
                repository.Delete(savedCustomer);
                scope.Commit();
            }

            using (var scope = new UnitOfWorkScope())
            {
                var repository = new EFRepository<Customer>();
                Assert.That(repository.FirstOrDefault(x => x.CustomerID == customer.CustomerID), Is.Null);
                scope.Commit();
            }
        }

        [Test]
        public void Can_attach()
        {
            Customer customer = null;
            var testData = new EFTestData(Context);
            testData.Batch(x => customer = x.CreateCustomer());
            Context.Detach(customer);
            Context.Dispose();

            using (var scope = new UnitOfWorkScope())
            {
                var repository = new EFRepository<Customer>();
                repository.Attach(customer);
                customer.FirstName = "Changed";
                scope.Commit();
            }

            Context = new PocoContext(ConnectionString);
            testData = new EFTestData(Context);
            customer = testData.Get<Customer>(x => x.CustomerID == customer.CustomerID);
            Assert.That(customer.FirstName, Is.EqualTo("Changed"));
        }

        [Test]
        public void Can_attach_modified_entity()
        {
            Customer customer = null;
            var testData = new EFTestData(Context);
            testData.Batch(x => customer = x.CreateCustomer());
            Context.Detach(customer);
            Context.Dispose();

            using (var scope = new UnitOfWorkScope())
            {
                customer.LastName = "Changed";
                var repository = new EFRepository<Customer>();
                repository.Attach(customer);
                scope.Commit();
            }

            Context = new PocoContext(ConnectionString);
            testData = new EFTestData(Context);
            customer = testData.Get<Customer>(x => x.CustomerID == customer.CustomerID);
            Assert.That(customer.LastName, Is.EqualTo("Changed"));
        }

        [Test]
        public void Can_query_using_specification()
        {
            var testData = new EFTestData(Context);
            testData.Batch(x =>
            {
                x.CreateCustomer(customer => customer.State = "CA");
                x.CreateCustomer(customer => customer.State = "CA");
                x.CreateCustomer(customer => customer.State = "PA");
            });
            
            using (var scope = new UnitOfWorkScope())
            {
                var specification = new Specification<Customer>(x => x.State == "CA");
                var results = new EFRepository<Customer>()
                    .Query(specification);
                Assert.That(results.Count(), Is.EqualTo(2));
                scope.Commit();
            }
        }

        [Test]
        public void Can_lazyload()
        {
            Customer customer = null;
            var testData = new EFTestData(Context);
            testData.Batch(x =>
            {
                customer = x.CreateCustomer();
                x.CreateOrderForCustomer(customer);
            });

            using (var scope = new UnitOfWorkScope())
            {
                var savedCustomer = new EFRepository<Customer>()
                    .First(x => x.CustomerID == customer.CustomerID);
                Assert.That(savedCustomer, Is.Not.Null);
                Assert.That(savedCustomer.Orders.Count, Is.EqualTo(1));
                scope.Commit();
            }
        }

        [Test]
        public void throws_when_lazyloading_outside_of_scope()
        {
            Order order = null;
            var testData = new EFTestData(Context);
            testData.Batch(x =>
                order = x.CreateOrderForCustomer(x.CreateCustomer()));

            Order savedOrder = null;
            using (var scope = new UnitOfWorkScope())
            {
                savedOrder = new EFRepository<Order>()
                    .First(x => x.OrderID == order.OrderID);
                scope.Commit();
            }

            Assert.That(savedOrder, Is.Not.Null);
            Assert.Throws<ObjectDisposedException>(() => { var fname = savedOrder.Customer.FirstName; });
        }
    }
}
