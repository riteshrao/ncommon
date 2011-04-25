using System.Linq;
using NCommon.Data.LinqToSql.Tests.OrdersDomain;
using NCommon.Extensions;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.LinqToSql.Tests
{
    [TestFixture]
    public class LinqToSqlRepositoryEagerFetchingTests : LinqToSqlRepositoryTestBase
    {
        [Test]
        public void Can_eager_fetch()
        {
            using (var testData = new LinqToSqlTestData(OrdersContextProvider()))
            {
                Order order = null;
                Order savedOrder = null;

                testData.Batch(x => order = x.CreateOrderForCustomer(x.CreateCustomer()));

                using (var scope = new UnitOfWorkScope())
                {
                    savedOrder = new LinqToSqlRepository<Order>()
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
            using (var testData = new LinqToSqlTestData(OrdersContextProvider()))
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
                    savedCustomer = new LinqToSqlRepository<Customer>()
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
            Locator.Stub(x => x.GetAllInstances<IFetchingStrategy<Customer, LinqToSqlRepositoryEagerFetchingTests>>())
                .Return(new[] { new FetchingStrategy() });

            using (var testData = new LinqToSqlTestData(OrdersContextProvider()))
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
                    savedCustomer = new LinqToSqlRepository<Customer>()
                        .For<LinqToSqlRepositoryEagerFetchingTests>()
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

        class FetchingStrategy : IFetchingStrategy<Customer, LinqToSqlRepositoryEagerFetchingTests>
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