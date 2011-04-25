using System.Linq;
using NCommon.Data.NHibernate.Tests.OrdersDomain;
using NHibernate;
using NHibernate.Linq;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.NHibernate.Tests
{
    public class NHRepositoryEagerFetchingTests : NHRepositoryTestBase
    {
        [Test]
        public void Can_eager_fetch()
        {
            using (var testData = new NHTestData(OrdersDomainFactory.OpenSession()))
            {
                Order order = null;
                Order savedOrder = null;

                testData.Batch(x => order = x.CreateOrderForCustomer(x.CreateCustomer()));

                using (var scope = new UnitOfWorkScope())
                {
                    savedOrder = new NHRepository<Order>()
                        .Where(x => x.OrderID == order.OrderID)
                        .Fetch(o => o.Customer)
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
            using (var testData = new NHTestData(OrdersDomainFactory.OpenSession()))
            {
                Customer customer = null;
                Customer savedCustomer = null;
                testData.Batch(x =>
                {
                    customer = x.CreateCustomer();
                    var order = x.CreateOrderForCustomer(customer);
                    order.Items.Add(x.CreateItem(order, x.CreateProduct()));
                    order.Items.Add(x.CreateItem(order, x.CreateProduct()));
                    order.Items.Add(x.CreateItem(order, x.CreateProduct()));
                });

                using (var scope = new UnitOfWorkScope())
                {
                    savedCustomer = new NHRepository<Customer>()
                        .Where(x => x.CustomerID == customer.CustomerID)
                        .FetchMany(x => x.Orders)
                        .ThenFetchMany(x => x.Items)
                        .ThenFetch(x => x.Product)
                        .SingleOrDefault();
                    scope.Commit();
                }

                Assert.NotNull(savedCustomer);
                Assert.True(NHibernateUtil.IsInitialized(savedCustomer.Orders));
                savedCustomer.Orders.ForEach(order =>
                {
                    Assert.True(NHibernateUtil.IsInitialized(order.Items));
                    order.Items.ForEach(orderItem => NHibernateUtil.IsInitialized(orderItem.Product));
                });
            }
        }

        [Test]
        public void Can_eager_fetch_using_for()
        {
            Locator.Stub(x => x.GetAllInstances<IFetchingStrategy<Customer, NHRepositoryEagerFetchingTests>>())
                .Return(new[] { new FetchingStrategy() });

    
            using (var testData = new NHTestData(OrdersDomainFactory.OpenSession()))
            {
                Customer customer = null;
                Customer savedCustomer = null;
                testData.Batch(x =>
                {
                    customer = x.CreateCustomer();
                    var order = x.CreateOrderForCustomer(customer);
                    order.Items.Add(x.CreateItem(order, x.CreateProduct()));
                    order.Items.Add(x.CreateItem(order, x.CreateProduct()));
                    order.Items.Add(x.CreateItem(order, x.CreateProduct()));
                });

                using (var scope = new UnitOfWorkScope())
                {
                    savedCustomer = new NHRepository<Customer>()
                        .For<NHRepositoryEagerFetchingTests>()
                        .Where(x => x.CustomerID == customer.CustomerID)
                        .SingleOrDefault();

                    scope.Commit();
                }

                Assert.NotNull(savedCustomer);
                Assert.NotNull(savedCustomer.Orders);
                savedCustomer.Orders.ForEach(order =>
                {
                    Assert.NotNull(order.Items);
                    order.Items.ForEach(orderItem => Assert.NotNull(orderItem.Product));
                });
            }
        }

        class FetchingStrategy : IFetchingStrategy<Customer, NHRepositoryEagerFetchingTests>
        {
            public IQueryable<Customer> Define(IRepository<Customer> repository)
            {
                return repository.FetchMany(x => x.Orders)
                    .ThenFetchMany(x => x.Items)
                    .ThenFetch(x => x.Product);
            }
        }
    }
}