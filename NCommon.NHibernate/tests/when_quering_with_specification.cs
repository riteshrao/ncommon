using System;
using System.Collections.Generic;
using System.Linq;
using NCommon.Extensions;
using NCommon.NHibernate.Tests.Domain;
using NCommon.Specifications;
using NCommon.Storage;
using NHibernate;
using NUnit.Framework;

namespace NCommon.Data.NHibernate.Tests
{
	[TestFixture]
	public class when_quering_with_specification : NHTestBase
	{
		public override void SetUp()
		{
			base.SetUp();
			var orderA = new CustomOrder
			{
				Id = 10,
				OrderDate = DateTime.Now,
			};
			orderA.Items = new HashSet<CustomOrderItem>
			{
				new CustomOrderItem {Name = "Tea", Store = "Internet", Order = orderA},
				new CustomOrderItem {Name = "Jamaican Coffee", Store = "Catalog", Order = orderA},
				new CustomOrderItem {Name = "Cordless Phone", Store = "Catalog", Order = orderA},
				new CustomOrderItem {Name = "Laptop", Store = "Internet", Order = orderA}
			};

			var orderB = new CustomOrder
			{
				Id = 20,
				OrderDate = DateTime.Now,

			};
			orderB.Items = new HashSet<CustomOrderItem>
			{
				new CustomOrderItem {Name = "Leather Wallet", Store = "Store", Order = orderB},
				new CustomOrderItem {Name = "PostIt Notes", Store = "Store", Order = orderB},
				new CustomOrderItem {Name = "Coasters", Store = "Catalog", Order = orderB},
				new CustomOrderItem {Name = "20\" Monitor", Store = "Catalog", Order = orderB}
			};

			var factory = Store.Local.Get<ISessionFactory>("NHRepositoryTests.SessionFactory");
			using (var session = factory.OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.CreateSQLQuery("DELETE CustomOrderItem").ExecuteUpdate();
				session.CreateSQLQuery("DELETE CustomOrders").ExecuteUpdate();
				session.Save(orderA);
				session.Save(orderB);
				tx.Commit();
			}
		}

		public override void TearDown()
		{
			using (var session = Factory.OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var orders = session.CreateCriteria(typeof(CustomOrder)).List<CustomOrder>();
				orders.ForEach(session.Delete);
				tx.Commit();
			}
			base.TearDown();
		}

		[Test]
		public void specification_query_returns_only_orders_containing_items_from_internet()
		{
			using (var scope = new UnitOfWorkScope())
			{
				var repository = new NHRepository<CustomOrder>();
				var specification = new Specification<CustomOrder>(
					order => order.Items.Any(x => x.Store == "Internet")
					);
				var results = (from order in repository.Query(specification)
							   select order).ToList();

				Assert.That(results, Is.Not.Null);
				Assert.That(results.Count, Is.EqualTo(1));
				Assert.That(results.First().Items.Where(x => x.Store == "Internet").Count(), Is.EqualTo(2));
				scope.Commit();
			}
		}
	}
}