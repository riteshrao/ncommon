using FluentNHibernate.Mapping;
using NCommon.NHibernate.Tests.Domain;

namespace NCommon.Data.NHibernate.Tests.Mappings
{
	public class CustomOrderItemMap : ClassMap<CustomOrderItem>
	{
		public CustomOrderItemMap()
		{
			Id(x => x.Id)
				.GeneratedBy.Identity();
			Map(x => x.Name);
			Map(x => x.Store);
			References(x => x.Order)
				.Column("CustomOrderId");
		}
	}
}