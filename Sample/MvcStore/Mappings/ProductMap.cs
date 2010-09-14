using MvcStore.Models;

namespace MvcStore.Mappings
{
    public class ProductMap : EntityMap<Product>
    {
        public ProductMap()
        {
            Map(x => x.Name)
                .Index("Idx_Product_Name");
            Map(x => x.Price);
            Map(x => x.ProductCode)
                .Index("Idx_Product_Code");
            Map(x => x.Description);
            References(x => x.Category)
                .ForeignKey("FK_Product_Category")
                .Column("CategoryId")
                .Index("Idx_Product_Category");
        }
    }
}