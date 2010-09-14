using System;
using System.Collections.Generic;
using System.Linq;
using MvcStore.Models;

namespace MvcStore.ViewModels
{
    public class CatalogViewModel
    {
        public CatalogViewModel(ShoppingCart cart, IEnumerable<string> categories, IEnumerable<Product> products)
        {
            TotalItemsInCart = cart.Items.Count();
            TotalCartValue = cart.Total;
            Categories = categories;
            CategoryProducts = products.Select(product => new ProductViewModel(product));
        }

        public int TotalItemsInCart { get; set; }
        public float TotalCartValue { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public IEnumerable<ProductViewModel> CategoryProducts { get; set; }
    }
}