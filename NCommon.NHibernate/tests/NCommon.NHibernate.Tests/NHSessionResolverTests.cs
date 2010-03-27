using System;
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
                .Mappings(mappings => mappings.FluentMappings.AddFromAssembly(typeof(SalesPerson).Assembly))
                .BuildSessionFactory();
        }

        [Test]
        public void RegisterSessionFactoryProvider_throws_ArgumentNullException_when_provider_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new NHSessionResolver().RegisterSessionFactoryProvider(null));
        }

        [Test]
        public void SessionFactoriesRegistered_returns_correct_count()
        {
            var resolver = new NHSessionResolver();
            resolver.RegisterSessionFactoryProvider(() => _ordersFactory);
            resolver.RegisterSessionFactoryProvider(() => _hrFactory);
            Assert.That(resolver.SessionFactoriesRegistered, Is.EqualTo(2));
        }

        [Test]
        public void GetSessionKeyFor_throws_ArgumentException_when_no_factory_is_registered_to_handle_specified_type()
        {
            Assert.Throws<ArgumentException>(() => new NHSessionResolver().GetSessionKeyFor<string>());
        }
        
        [Test]
        public void GetSessionKeyFor_returns_same_key_for_types_handled_by_the_same_factory()
        {
            var resolver = new NHSessionResolver();
            resolver.RegisterSessionFactoryProvider(() => _ordersFactory);
            resolver.RegisterSessionFactoryProvider(() => _hrFactory);

            var key = resolver.GetSessionKeyFor<Customer>();
            var key2 = resolver.GetSessionKeyFor<Order>();
            Assert.That(key, Is.EqualTo(key2));
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

            var resolved = resolver.GetFactoryFor<SalesPerson>();
            Assert.That(resolved, Is.Not.Null);
            Assert.That(resolved, Is.SameAs(_hrFactory));
        }
    }
}