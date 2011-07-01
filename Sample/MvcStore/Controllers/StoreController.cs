using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MvcStore.Services;
using MvcStore.ViewModels;
using NCommon.Mvc;
using Product = MvcStore.Models.Product;

namespace MvcStore.Controllers
{
    public class StoreController : ControllerBase
    {
        readonly IProductCatalog _productCatalog;
        readonly IShoppingCartLocator _shoppingCartLocator;

        public StoreController(IProductCatalog productCatalog, IShoppingCartLocator shoppingCartLocator)
        {
            _productCatalog = productCatalog;
            _shoppingCartLocator = shoppingCartLocator;
        }

        [UnitOfWork]
        public ActionResult Index()
        {
            var cart = _shoppingCartLocator.GetCart();
            return View(new CatalogViewModel(cart, _productCatalog.GetCategoryNames(), new Product[] {}));
        }

        [UnitOfWork(Scope = UnitOfWorkAttribute.FilterScope.Result)]
        public ActionResult DisplayCategory(string categoryName)
        {
            return View(new CatalogViewModel(
                            _shoppingCartLocator.GetCart(),
                            _productCatalog.GetCategoryNames(),
                            _productCatalog.GetProductsForCategory(categoryName)));
        }

        [UnitOfWork]
        public ActionResult AddToCart(string category, string productCode)
        {
            var cart = _shoppingCartLocator.GetCart();
            var product = _productCatalog.GetProduct(category, productCode);
            cart.AddToCart(product);

            return View(new CatalogViewModel(
                     cart,
                     _productCatalog.GetCategoryNames(),
                     _productCatalog.GetProductsForCategory(category)));
        }

        [UnitOfWork]
        public ActionResult ViewCart()
        {
            return View(new ShoppingCartViewModel(
                            _shoppingCartLocator.GetCart(),
                            _productCatalog.GetCategoryNames()));
        }

        [UnitOfWork]
        public ActionResult ShowCart()
        {
            return View(new ShoppingCartViewModel(
                _shoppingCartLocator.GetCart(),
                _productCatalog.GetCategoryNames()));
        }

        [UnitOfWork]
        public ActionResult RemoveCartItem(Guid itemId)
        {
            var cart = _shoppingCartLocator.GetCart();
            cart.RemoveFromCart(itemId);
            return View(new ShoppingCartViewModel(cart, _productCatalog.GetCategoryNames()));
        }

        [UnitOfWork]
        public ActionResult UpdateCart(List<ShoppingCartItemViewModel> items)
        {
            var cart = _shoppingCartLocator.GetCart();
            items.ForEach(item => cart.SetQuantity(item.Id, item.Quantity));
            return View(new ShoppingCartViewModel(cart, _productCatalog.GetCategoryNames()));
        }
    }
}