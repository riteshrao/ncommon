using FluentNHibernate.Mapping;
using MvcStore.Models;

namespace MvcStore.Mappings
{
    public class ShoppingCartItemMap : EntityMap<ShoppingCartItem>
    {
        public ShoppingCartItemMap()
        {
            Map(x => x.ProductId);
            Map(x => x.ProductCode);
            Map(x => x.ProductName);
            Map(x => x.ProductCategory);
            Map(x => x.Price);
            Map(x => x.Quantity);
            References(x => x.ShoppingCart)
                .Column("ShoppingCartId")
                .ForeignKey("FK_ShoppingCartItem_ShoppingCart")
                .Index("Idx_ShoppingCart_Id")
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore);
        }
    }
}