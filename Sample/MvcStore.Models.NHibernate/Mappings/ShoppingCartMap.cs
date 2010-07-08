using FluentNHibernate.Mapping;

namespace MvcStoreModels.NHibernate.Mappings
{
    public class ShoppingCartMap : ClassMap<ShoppingCart>
    {
        public ShoppingCartMap()
        {
            Table("ShoppingCarts");
            Id(x => x.Id)
                .GeneratedBy.Assigned()
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore);
            Map(x => x.Created)
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore);
            Map(x => x.LastModified)
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore);
            HasMany(x => x.Items)
                .AsSet()
                .Inverse()
                .Cascade.AllDeleteOrphan()
                .KeyColumn("CartId")
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore);
        }
    }
}