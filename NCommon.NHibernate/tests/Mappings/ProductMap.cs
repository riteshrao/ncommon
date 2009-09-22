using FluentNHibernate.Mapping;
using NCommon.Data.NHibernate.Tests.Domain;

namespace NCommon.Data.NHibernate.Tests.Mappings
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