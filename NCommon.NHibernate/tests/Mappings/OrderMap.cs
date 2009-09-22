using FluentNHibernate.Mapping;
using NCommon.Data.NHibernate.Tests.Domain;

namespace NCommon.Data.NHibernate.Tests.Mappings
{
	public class OrderMap : ClassMap<Order>
	{
		public OrderMap()
		{
			Table("Orders");
			Id(x => x.OrderID)
				.GeneratedBy.Identity();
			Map(x => x.OrderDate);
			Map(x => x.ShipDate);
			References(x => x.Customer, "CustomerId")
				.ForeignKey("FK_Orders_Customer");
			HasMany(x => x.Items)
				.AsSet()
				.Inverse()
				.Cascade.AllDeleteOrphan()
				.KeyColumn("OrderId")
				.ForeignKeyConstraintName("FK_Orders_OrderItems");
		}
	}
}