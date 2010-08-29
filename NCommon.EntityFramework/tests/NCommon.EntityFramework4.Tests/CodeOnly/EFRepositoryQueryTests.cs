using System;
using System.Data.Objects;
using System.Linq;
using System.Data.Entity.Infrastructure;
using Microsoft.Practices.ServiceLocation;
using NCommon.Data;
using NCommon.Data.EntityFramework;
using NCommon.EntityFramework4.Tests.Models;
using NCommon.Extensions;
using NCommon.Specifications;
using NCommon.State;
using NCommon.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.EntityFramework4.Tests.CodeOnly
{
    [TestFixture]
    public class EFRepositoryQueryTests
    {
        private IState _state;
        private IServiceLocator _locator;
        private ObjectContext _context;
        private EFUnitOfWorkFactory _unitOfWorkFactory;

        public class NullInitializer : IDatabaseInitializer<CodeOnlyContext>
        {
            public void InitializeDatabase(CodeOnlyContext context)
            {
                
            }
        }

        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
            _unitOfWorkFactory = new EFUnitOfWorkFactory();
            Database.SetInitializer(new NullInitializer());
            _unitOfWorkFactory.RegisterObjectContextProvider(() => new CodeOnlyContext("SandboxCodeOnly").Context);

            _locator = MockRepository.GenerateStub<IServiceLocator>();
            _locator.Stub(x => x.GetInstance<IUnitOfWorkFactory>()).Return(_unitOfWorkFactory);
            _locator.Stub(x => x.GetInstance<IState>()).Do(new Func<IState>(() => _state));
            ServiceLocator.SetLocatorProvider(() => _locator);
        }

        [SetUp]
        public virtual void TestSetup()
        {
            _state = new FakeState();
            _context = new CodeOnlyContext("SandboxCodeOnly").Context;
        }

        [TearDown]
        public void TestTeardown()
        {
            _context.ExecuteStoreCommand("DELETE OrderItems");
            _context.ExecuteStoreCommand("DELETE Products");
            _context.ExecuteStoreCommand("DELETE Orders");
            _context.ExecuteStoreCommand("DELETE Customers");
            _context.Dispose();
        }

        [Test]
        public void Can_perform_simple_query()
        {
            var testData = new EFTestData(_context);
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
                new EFRepository<Customer>().Save(customer);
                scope.Commit();
                customerId = customer.CustomerID;
            }
            var savedCustomer = new EFTestData(_context)
                .Get<Customer>(x => x.CustomerID == customerId);
            Assert.That(savedCustomer, Is.Not.Null);
        }

        [Test]
        public void Can_modify()
        {
            Customer customer = null;
            var testData = new EFTestData(_context);
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
            var testData = new EFTestData(_context);
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
            var testData = new EFTestData(_context);
            testData.Batch(x => customer = x.CreateCustomer());
            _context.Detach(customer);
            _context.Dispose();

            using (var scope = new UnitOfWorkScope())
            {
                var repository = new EFRepository<Customer>();
                repository.Attach(customer);
                customer.FirstName = "Changed";
                scope.Commit();
            }

            _context = new CodeOnlyContext("SandboxCodeOnly").Context;
            testData = new EFTestData(_context);
            customer = testData.Get<Customer>(x => x.CustomerID == customer.CustomerID);
            Assert.That(customer.FirstName, Is.EqualTo("Changed"));
        }

        [Test]
        public void Can_query_using_specification()
        {
            var testData = new EFTestData(_context);
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
            var testData = new EFTestData(_context);
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
            var testData = new EFTestData(_context);
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

        [Test]
        public void Can_eger_fetch_using_eagerly()
        {
            Customer customer = null;
            var testData = new EFTestData(_context);
            testData.Batch(x =>
            {
                customer = x.CreateCustomer();
                x.CreateOrderForCustomer(customer);
                x.CreateOrderForCustomer(customer);
            });

            Customer savedCustomer;
            using (var scope = new UnitOfWorkScope())
            {
                savedCustomer = new EFRepository<Customer>()
                    .Eagerly(f => f.Fetch<Order>(x => x.Orders)
                                      .And<OrderItem>(x => x.OrderItems)
                                      .And<Product>(x => x.Product))
                    .First(x => x.CustomerID == customer.CustomerID);
                scope.Commit();
            }

            Assert.That(savedCustomer, Is.Not.Null);
            Assert.That(savedCustomer.Orders, Is.Not.Null);
            Assert.That(savedCustomer.Orders.Count(), Is.EqualTo(2));
            savedCustomer.Orders.ForEach(order =>
            {
                Assert.That(order.OrderItems, Is.Not.Null);
                Assert.That(order.OrderItems.Count(), Is.GreaterThan(0));
                order.OrderItems.ForEach(item => Assert.That(item.Product, Is.Not.Null));
            });
        }
    }
}