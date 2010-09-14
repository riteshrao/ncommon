using System;
using MvcStore.Models;

namespace MvcStore.ViewModels
{
    public class ProductViewModel
    {
        public ProductViewModel(Product product)
        {
            Id = product.Id;
            Category = product.Category.Name;
            Code = product.ProductCode;
            Name = product.Name;
            Price = product.Price;
            Thumbnail = GetThumbnailUrl(product.ProductCode);
        }

        public ProductViewModel(ShoppingCartItem cartItem)
        {
            Id = cartItem.ProductId;
            Category = cartItem.ProductCategory;
            Name = cartItem.ProductName;
            Code = cartItem.ProductCode;
            Thumbnail = GetThumbnailUrl(cartItem.ProductCode);
            Price = cartItem.Price;
        }

        public Guid Id { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Thumbnail { get; set; }
        public float Price { get; set; }

        static string GetThumbnailUrl(string productCode)
        {
            return "/Content/productimages/" + productCode + "_Thumbnail.jpg";
        }
    }
}