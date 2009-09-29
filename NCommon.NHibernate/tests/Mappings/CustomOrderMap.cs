using FluentNHibernate.Mapping;
using NCommon.NHibernate.Tests.Domain;

namespace NCommon.Data.NHibernate.Tests.Mappings
{
	public class CustomOrderMap : ClassMap<CustomOrder>
	{
		public CustomOrderMap()
		{
			Table("CustomOrders");
			Id(x => x.Id)
				.GeneratedBy.Assigned();
			Map(x => x.OrderDate);
			HasMany(x => x.Items)
				.AsSet()
				.Inverse()
				.Cascade.AllDeleteOrphan()
				.KeyColumn("CustomOrderId");
		}
	}
}