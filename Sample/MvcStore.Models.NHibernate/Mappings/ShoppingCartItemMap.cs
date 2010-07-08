using FluentNHibernate.Mapping;

namespace MvcStoreModels.NHibernate.Mappings
{
    public class ShoppingCartItemMap : ClassMap<ShoppingCartItem>
    {
        public ShoppingCartItemMap()
        {
            Table("ShoppingCartItems");
            Id(x => x.Id)
                .GeneratedBy.Assigned()
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore);
            Map(x => x.ProductId);
            Map(x => x.ProductName);
            Map(x => x.Quantity);
            Map(x => x.Price);
            References(x => x.Cart)
                .Column("CartId")
                .ForeignKey("FK_ShoppingCartItems_ShoppingCart")
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore);
        }
    }
}