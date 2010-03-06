using FluentNHibernate.Mapping;

namespace NCommon.Data.NHibernate.Tests.OrdersDomain.Mappings
{
	public class ProductMap : ClassMap<Product>
	{
		public ProductMap()
		{
			Table("Products");
			Id(x => x.ProductID)
				.GeneratedBy.Identity();
			Map(x => x.Name);
			Map(x => x.Description);
		}
	}
}