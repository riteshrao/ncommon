using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NCommon.Data.NHibernate.Tests.OrdersDomain;
using NCommon.Data.NHibernate.Tests.HRDomain.Domain;
using NCommon.Util;
using NHibernate;
using NHibernate.ByteCode.Castle;
using NUnit.Framework;

namespace NCommon.Data.NHibernate.Tests
{
    [TestFixture]
    public class NHSessionResolverTests
    {
        ISessionFactory _ordersFactory;
        ISessionFactory _hrFactory;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            _ordersFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2005
                              .ConnectionString(connection => connection.FromConnectionStringWithKey("testdb"))
                              .ProxyFactoryFactory(typeof (ProxyFactoryFactory)))
                .Mappings(mappings => mappings.FluentMappings.AddFromAssembly(typeof (Order).Assembly))
                .BuildSessionFactory();

            _hrFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2005
                              .ConnectionString(connection => connection.FromConnectionStringWithKey("testdb"))
                              .ProxyFactoryFactory(typeof(ProxyFactoryFactory)))
                .Mappings(mappings => mappings.FluentMappings.AddFromAssembly(typeof(Employee).Assembly))
                .BuildSessionFactory();
        }

        [Test]
        public void GetSessionFactoryFor_returns_orders_factory_when_requested_for_customer()
        {
            var resolver = new NHSessionResolver();
            resolver.RegisterSessionFactoryProvider(() => _ordersFactory);
            resolver.RegisterSessionFactoryProvider(() => _hrFactory);

            var resolved = resolver.GetFactoryFor<Order>();
            Assert.That(resolved, Is.Not.Null);
            Assert.That(resolved, Is.SameAs(_ordersFactory));
        }

        [Test]
        public void GetSessionFactoryFor_returns_hr_factory_when_requested_for_Employee()
        {
            var resolver = new NHSessionResolver();
            resolver.RegisterSessionFactoryProvider(() => _ordersFactory);
            resolver.RegisterSessionFactoryProvider(() => _hrFactory);

            var resolved = resolver.GetFactoryFor<Employee>();
            Assert.That(resolved, Is.Not.Null);
            Assert.That(resolved, Is.SameAs(_hrFactory));
        }
    }
}