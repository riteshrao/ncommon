using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MvcStore.Models;
using MvcStore.ViewModels;
using MvcStoreModels.Services;
using NCommon.Data;

namespace MvcStore.Controllers
{
    public class StoreController : Controller
    {
        readonly IProductCatalog _productCatalog;
        readonly IShoppingCartLocator _shoppingCartLocator;

        public StoreController(IProductCatalog productCatalog, IShoppingCartLocator shoppingCartLocator)
        {
            _productCatalog = productCatalog;
            _shoppingCartLocator = shoppingCartLocator;
        }

        public ActionResult Index()
        {
            var viewModel = new StoreViewModel();
            using (var scope = new UnitOfWorkScope())
            {
                var categories = _productCatalog.GetAllCategories();
                var selectedCategory = categories.First();
                var products = _productCatalog.GetProductsForCategory(selectedCategory.Id);

                viewModel.CurrentCategory = selectedCategory.Name;
                viewModel.Categories = categories.Select(x => x.Name).ToList();
                viewModel.Products = products.Select(x => new ProductSummary
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToArray();
                scope.Commit();
            }
            return View("catalog", viewModel);
        }

        public ActionResult ShowCategoryProducts(string category)
        {
            var viewModel = new StoreViewModel();
            using (var scope = new UnitOfWorkScope())
            {
                var categories = _productCatalog.GetAllCategories();
                var selectedCategory = categories.Where(x => x.Name.Equals(category)).First();
                var products = _productCatalog.GetProductsForCategory(selectedCategory.Id);
                viewModel.CurrentCategory = selectedCategory.Name;
                viewModel.Categories = categories.Select(x => x.Name).ToList();
                viewModel.Products = products.Select(x => new ProductSummary
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToArray();
                scope.Commit();
            }
            return View("catalog", viewModel);
        }
    }
}