using System.Linq;
using NCommon.Data;
using NCommon.Data.EntityFramework;
using NCommon.EntityFramework4.Tests.Models;
using NCommon.Extensions;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.EntityFramework4.Tests.POCO
{
    [TestFixture]
    public class EFRepositoryEagerFetchingTests : EFRepositoryQueryTestsBase
    {
        [Test]
        public void Can_eager_fetch()
        {
            var testData = new EFTestData(Context);

            Order order = null;
            Order savedOrder = null;

            testData.Batch(x => order = x.CreateOrderForCustomer(x.CreateCustomer()));

            using (var scope = new UnitOfWorkScope())
            {
                savedOrder = new EFRepository<Order>()
                    .Fetch(o => o.Customer)
                    .Where(x => x.OrderID == order.OrderID)
                    .SingleOrDefault();
                scope.Commit();
            }

            Assert.NotNull(savedOrder);
            Assert.NotNull(savedOrder.Customer);
            Assert.DoesNotThrow(() => { var firstName = savedOrder.Customer.FirstName; });

        }

        [Test]
        public void Can_eager_fetch_many()
        {
            var testData = new EFTestData(Context);

            Customer customer = null;
            Customer savedCustomer = null;
            testData.Batch(x =>
            {
                customer = x.CreateCustomer();
                var order = x.CreateOrderForCustomer(customer);
                order.OrderItems.Add(x.CreateOrderItem(item => item.Order = order));
                order.OrderItems.Add(x.CreateOrderItem(item => item.Order = order));
                order.OrderItems.Add(x.CreateOrderItem(item => item.Order = order));
            });

            using (var scope = new UnitOfWorkScope())
            {
                savedCustomer = new EFRepository<Customer>()
                    .FetchMany(x => x.Orders)
                    .ThenFetchMany(x => x.OrderItems)
                    .ThenFetch(x => x.Product)
                    .Where(x => x.CustomerID == customer.CustomerID)
                    .SingleOrDefault();
                scope.Commit();
            }

            Assert.NotNull(savedCustomer);
            Assert.NotNull(savedCustomer.Orders);
            savedCustomer.Orders.ForEach(order =>
            {
                Assert.NotNull(order.OrderItems);
                order.OrderItems.ForEach(orderItem => Assert.NotNull(orderItem.Product));
            });
        }

        [Test]
        public void Can_eager_fetch_using_for()
        {
            Locator.Stub(x => x.GetAllInstances<IFetchingStrategy<Customer, EFRepositoryEagerFetchingTests>>())
                .Return(new[] { new FetchingStrategy() });

            var testData = new EFTestData(Context);
            Customer customer = null;
            Customer savedCustomer = null;
            testData.Batch(x =>
            {
                customer = x.CreateCustomer();
                var order = x.CreateOrderForCustomer(customer);
                order.OrderItems.Add(x.CreateOrderItem(item => item.Order = order));
                order.OrderItems.Add(x.CreateOrderItem(item => item.Order = order));
                order.OrderItems.Add(x.CreateOrderItem(item => item.Order = order));
            });

            using (var scope = new UnitOfWorkScope())
            {
                savedCustomer = new EFRepository<Customer>()
                    .For<EFRepositoryEagerFetchingTests>()
                    .Where(x => x.CustomerID == customer.CustomerID)
                    .SingleOrDefault();
                scope.Commit();
            }

            Assert.NotNull(savedCustomer);
            Assert.NotNull(savedCustomer.Orders);
            savedCustomer.Orders.ForEach(order =>
            {
                Assert.NotNull(order.OrderItems);
                order.OrderItems.ForEach(orderItem => Assert.NotNull(orderItem.Product));
            });
        }

        class FetchingStrategy : IFetchingStrategy<Customer, EFRepositoryEagerFetchingTests>
        {
            public IRepository<Customer> Define(IRepository<Customer> repository)
            {
                return repository.FetchMany(x => x.Orders)
                    .ThenFetchMany(x => x.OrderItems)
                    .ThenFetch(x => x.Product);
            }
        }
    }
}