using System;
using System.Collections.Generic;
using System.Linq;
using NCommon.Data;
using NCommon.State;

namespace MvcStoreModels.Services
{
    public class ProductCatalog : IProductCatalog
    {
        readonly ICacheState _cache;
        readonly IRepository<Category> _categoriesRepository;
        readonly IRepository<Product> _productsRepository;

        public ProductCatalog(
            ICacheState cache, 
            IRepository<Category> categoriesRepository, 
            IRepository<Product> productsRepository)
        {
            _cache = cache;
            _productsRepository = productsRepository;
            _categoriesRepository = categoriesRepository;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            var categories = _cache.Get<Category[]>();
            if (categories == null)
            {
                using (var scope = new UnitOfWorkScope())
                {
                    categories = _categoriesRepository
                        .OrderBy(x => x.Name)
                        .ToArray();
                    scope.Commit();
                }
                _cache.Put(categories);
            }
            return categories;
        }

        public IEnumerable<Product> GetProductsForCategory(Guid categoryId)
        {
            var products = _cache.Get<Product[]>(categoryId);
            if (products == null)
            {
                using (var scope = new UnitOfWorkScope())
                {
                    products = _productsRepository
                        .Where(x => x.Category.Id == categoryId)
                        .ToArray();
                    _cache.Put(categoryId, products, DateTime.Now.AddDays(1));
                    scope.Commit();
                }
            }
            return products;
        }

        public Product GetProduct(Guid categoryId, Guid productId)
        {
            var products = GetProductsForCategory(categoryId);
            return products.SingleOrDefault(x => x.Id == productId);
        }
    }
}