using FluentNHibernate.Mapping;

namespace MvcStoreModels.NHibernate.Mappings
{
    public class ProductMap : ClassMap<Product>
    {
        public ProductMap()
        {
            Table("Products");
            Id(x => x.Id)
                .GeneratedBy.Assigned()
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore);
            Map(x => x.Code).Unique();
            Map(x => x.Name);
            Map(x => x.Price);
            References(x => x.Category)
                .Column("CategoryId")
                .ForeignKey("FK_Product_Categories");
        }
    }
}