using FluentNHibernate.Mapping;
using MvcStore.Models;

namespace MvcStore.Mappings
{
    public class ShoppingCartMap : EntityMap<ShoppingCart>
    {
        public ShoppingCartMap()
        {
            Map(x => x.Created)
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore);
            Map(x => x.LastModified)
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore);
            HasMany(x => x.Items)
                .AsSet()
                .Inverse()
                .Cascade.AllDeleteOrphan()
                .KeyColumn("ShoppingCartId")
                .ForeignKeyConstraintName("FK_ShoppingCart_Items")
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore);
        }
    }
}