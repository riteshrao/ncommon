using System;
using System.Collections.Generic;
using MvcStore.Models;

namespace MvcStore.Services
{
    public interface IProductCatalog
    {
        IEnumerable<string> GetCategoryNames();
        IEnumerable<Category> GetAllCategories();
        IEnumerable<Product> GetProductsForCategory(string category);
        Product GetProduct(string category, string productCode);
    }
}