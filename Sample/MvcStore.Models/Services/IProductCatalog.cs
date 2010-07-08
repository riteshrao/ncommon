using System;
using System.Collections.Generic;

namespace MvcStoreModels.Services
{
    public interface IProductCatalog
    {
        IEnumerable<Category> GetAllCategories();
        IEnumerable<Product> GetProductsForCategory(Guid categoryId);
        Product GetProduct(Guid categoryId, Guid productId);
    }
}