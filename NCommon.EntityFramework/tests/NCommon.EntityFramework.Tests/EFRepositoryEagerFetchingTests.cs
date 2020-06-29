using System.Linq;
using NCommon.Data.EntityFramework.Tests.OrdersDomain;
using NCommon.DataServices.Transactions;
using NCommon.Extensions;
using NCommon.ObjectAccess;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.EntityFramework.Tests
{
    [TestFixture]
    public class EFRepositoryEagerFetchingTests : EFRepositoryTestBase
    {
        [Test]
        public void Can_eager_fetch()
        {
            using (var testData = new EFTestData(OrdersContextProvider()))
            {
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
        }
        
        [Test]
        public void Can_eager_fetch_many()
        {
            using (var testData = new EFTestData(OrdersContextProvider()))
            {
                Customer customer = null;
                Customer savedCustomer = null;
                testData.Batch(x =>
                {
                    customer = x.CreateCustomer();
                    var order = x.CreateOrderForCustomer(customer);
                    order.OrderItems.Add(x.CreateItem(order, x.CreateProduct()));
                    order.OrderItems.Add(x.CreateItem(order, x.CreateProduct()));
                    order.OrderItems.Add(x.CreateItem(order, x.CreateProduct()));
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
        }

        [Test]
        public void Can_eager_fetch_using_for()
        {
            Locator.Stub(x => x.GetAllInstances<IFetchingStrategy<Customer, EFRepositoryEagerFetchingTests>>())
                .Return(new[] {new FetchingStrategy()});

            using (var testData = new EFTestData(OrdersContextProvider()))
            {
                Customer customer = null;
                Customer savedCustomer = null;
                testData.Batch(x =>
                {
                    customer = x.CreateCustomer();
                    var order = x.CreateOrderForCustomer(customer);
                    order.OrderItems.Add(x.CreateItem(order, x.CreateProduct()));
                    order.OrderItems.Add(x.CreateItem(order, x.CreateProduct()));
                    order.OrderItems.Add(x.CreateItem(order, x.CreateProduct()));
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
        }

        class FetchingStrategy : IFetchingStrategy<Customer, EFRepositoryEagerFetchingTests>
        {
            public IQueryable<Customer> Define(IRepository<Customer> repository)
            {
                return repository.FetchMany(x => x.Orders)
                    .ThenFetchMany(x => x.OrderItems)
                    .ThenFetch(x => x.Product);
            }
        }
    }
}