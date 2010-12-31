using Microsoft.Practices.ServiceLocation;
using NCommon.Data.EntityFramework.Tests.OrdersDomain;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.EntityFramework.Tests
{
    [TestFixture]
    public class EFRepositoryExtensionsTests
    {
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            var mockLocator = MockRepository.GenerateStub<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => mockLocator);
        }

        [Test]
        public void Fetch_returns_a_fetching_repository_with_correct_path()
        {
            var repository = new EFRepository<Order>();
            var fetchingRepo = repository.Fetch(order => order.Customer);
            Assert.AreEqual("Customer", fetchingRepo.FetchingPath);
        }

        [Test]
        public void FetchMany_returns_a_fetching_repository_with_corrent_path()
        {
            var repository = new EFRepository<Customer>();
            var fetchingRepo = repository.FetchMany(customer => customer.Orders);
            Assert.AreEqual("Orders", fetchingRepo.FetchingPath);
        }

        [Test]
        public void Can_fectch_many_on_association()
        {
            var repository = new EFRepository<Order>();
            var fetchingRepo = repository
                .Fetch(order => order.Customer)
                .ThenFetchMany(customer => customer.Orders);

            Assert.AreEqual("Customer.Orders", fetchingRepo.FetchingPath);
        }

        [Test]
        public void Can_fetch_after_a_many_fetch()
        {
            var repository = new EFRepository<Order>();
            var fetchingRepo = repository
                .FetchMany(x => x.OrderItems)
                .ThenFetch(x => x.Product);
            Assert.AreEqual("OrderItems.Product", fetchingRepo.FetchingPath);
        }
    }
}