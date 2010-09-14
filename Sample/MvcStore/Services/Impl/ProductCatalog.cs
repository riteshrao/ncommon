using System;
using System.Collections.Generic;
using System.Linq;
using MvcStore.Models;
using NCommon.Data;
using NCommon.State;

namespace MvcStore.Services.Impl
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

        public IEnumerable<string> GetCategoryNames()
        {
            return GetAllCategories()
                .Select(x => x.Name);
        }

        public IEnumerable<Category> GetAllCategories()
        {
            var categories = _cache.Get<Category[]>();
            if (categories == null)
            {
                categories = _categoriesRepository
                    .OrderBy(x => x.Name)
                    .ToArray();
                _cache.Put(categories);
            }
            return categories;
        }

        public IEnumerable<Product> GetProductsForCategory(string category)
        {
            var products = _cache.Get<Product[]>(category);
            if (products == null)
            {
                products = _productsRepository
                    .Where(x => x.Category.Name == category)
                    .OrderBy(x => x.Name)
                    .ToArray();
                _cache.Put(products, category);
            }
            return products;
        }

        public Product GetProduct(string category, string productCode)
        {
            return GetProductsForCategory(category)
                .Where(x => x.ProductCode == productCode)
                .SingleOrDefault();
        }
    }
}