using FluentNHibernate.Mapping;
using NCommon.Data.NHibernate.Tests.Domain;

namespace NCommon.Data.NHibernate.Tests.Mappings
{
	public class CustomerMap : ClassMap<Customer>
	{
		public CustomerMap()
		{
			Table("Customers");
			Id(x => x.CustomerID)
				.GeneratedBy.Identity();
			Map(x => x.FirstName);
			Map(x => x.LastName);
			Component(x => x.Address, component =>
			{
				component.Map(x => x.StreetAddress1);
				component.Map(x => x.StreetAddress2);
				component.Map(x => x.City);
				component.Map(x => x.State);
				component.Map(x => x.ZipCode);
			});

			HasMany(x => x.Orders)
				.AsSet()
				.Inverse()
				.KeyColumn("CustomerId")
				.ForeignKeyConstraintName("FK_Customer_Orders");
		}
	}
}