using FluentNHibernate.Mapping;
using NCommon.Data.NHibernate.Tests.Domain;

namespace NCommon.Data.NHibernate.Tests.Mappings
{
	public class OrderItemMap : ClassMap<OrderItem>
	{
		public OrderItemMap()
		{
			Table("OrderItems");
			Id(x => x.OrderItemID)
				.GeneratedBy.Identity();
			Map(x => x.Price);
			Map(x => x.Quantity);
			Map(x => x.Store);
			References(x => x.Product)
				.Column("ProductId")
				.ForeignKey("FK_OrderItems_Product");
			References(x => x.Order)
				.Column("OrderId")
				.ForeignKey("FK_OrderItems_Order");
		}
	}
}