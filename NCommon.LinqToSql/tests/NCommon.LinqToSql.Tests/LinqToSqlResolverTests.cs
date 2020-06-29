using System;
using System.Configuration;
using NCommon.LinqToSql.Tests.HRDomain;
using NCommon.LinqToSql.Tests.OrdersDomain;
using NUnit.Framework;

namespace NCommon.LinqToSql.Tests
{
    public class LinqToSqlResolverTests
    {
        OrdersDataDataContext _ordersDataContext;
        HRDataDataContext _hrDataContext;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _ordersDataContext = new OrdersDataDataContext(ConfigurationManager.ConnectionStrings["testDb"].ConnectionString);
            _hrDataContext = new HRDataDataContext(ConfigurationManager.ConnectionStrings["testDb"].ConnectionString);
        }

        [Test]
        public void RegisterDataContextProvider_throws_ArgumentNullException_when_provider_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new LinqToSqlSessionResolver().RegisterDataContextProvider(null));
        }

        [Test]
        public void SessionFactoriesRegistered_returns_correct_count()
        {
            var resolver = new LinqToSqlSessionResolver();
            resolver.RegisterDataContextProvider(() => _ordersDataContext);
            resolver.RegisterDataContextProvider(() => _hrDataContext);
            Assert.That(resolver.DataContextsRegistered, Is.EqualTo(2));
        }

        [Test]
        public void GetSessionKeyFor_throws_ArgumentException_when_no_factory_is_registered_to_handle_specified_type()
        {
            Assert.Throws<ArgumentException>(() => new LinqToSqlSessionResolver().GetSessionKeyFor<string>());
        }

        [Test]
        public void GetSessionKeyFor_returns_same_key_for_types_handled_by_the_same_factory()
        {
            var resolver = new LinqToSqlSessionResolver();
            resolver.RegisterDataContextProvider(() => _ordersDataContext);
            resolver.RegisterDataContextProvider(() => _hrDataContext);

            var key = resolver.GetSessionKeyFor<Customer>();
            var key2 = resolver.GetSessionKeyFor<Order>();
            Assert.That(key, Is.EqualTo(key2));
        }

        [Test]
        public void GetSessionFactoryFor_returns_orders_factory_when_requested_for_customer()
        {
            var resolver = new LinqToSqlSessionResolver();
            resolver.RegisterDataContextProvider(() => _ordersDataContext);
            resolver.RegisterDataContextProvider(() => _hrDataContext);

            var resolved = resolver.GetDataContextFor<Order>();
            Assert.That(resolved, Is.Not.Null);
            Assert.That(resolved, Is.SameAs(_ordersDataContext));
        }

        [Test]
        public void GetSessionFactoryFor_returns_hr_factory_when_requested_for_Employee()
        {
            var resolver = new LinqToSqlSessionResolver();
            resolver.RegisterDataContextProvider(() => _ordersDataContext);
            resolver.RegisterDataContextProvider(() => _hrDataContext);

            var resolved = resolver.GetDataContextFor<SalesPerson>();
            Assert.That(resolved, Is.Not.Null);
            Assert.That(resolved, Is.SameAs(_hrDataContext));
        }
    }
}