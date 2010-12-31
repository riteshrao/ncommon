using System;
using System.Linq;
using System.Web;
using MvcStore.Models;
using NCommon.Context;
using NCommon.Data;

namespace MvcStore.Services.Impl
{
    public class ShppingCartLocator : IShoppingCartLocator
    {
        readonly IContext _context;
        readonly IRepository<ShoppingCart> _shoppingCartRepository;
        const string SHOPPING_CART_ID_COOKIE_NAME = "MVCStore_ShoppingCartId";

        public ShppingCartLocator(
            IContext context, 
            IRepository<ShoppingCart> shoppingCartRepository)
        {
            _context = context;
            _shoppingCartRepository = shoppingCartRepository;
        }

        public ShoppingCart GetCart()
        {
            ShoppingCart cart = null;
            var cartId = GetCartIdFromCookie();
            if (cartId != null)
                cart = LoadCart(cartId.Value);

            if (cart == null)
                cart = CreateCart();

            SetCartIdCookie(cart.Id);
            return cart;
        }

        ShoppingCart CreateCart()
        {
            var cart = new ShoppingCart();
            _shoppingCartRepository.Add(cart);
            return cart;
        }

        ShoppingCart LoadCart(Guid cartId)
        {
            return _shoppingCartRepository
                .Where(x => x.Id == cartId)
                .SingleOrDefault();
        }

        Guid? GetCartIdFromCookie()
        {
            var cookie = _context.HttpContext.Request.Cookies[SHOPPING_CART_ID_COOKIE_NAME];
            if (cookie == null)
                return null;
            Guid cartId;
            return !Guid.TryParse(cookie.Value, out cartId)
                       ? (Guid?) null
                       : cartId;
        }

        void SetCartIdCookie(Guid cartId)
        {
            _context.HttpContext
                .Response
                .Cookies
                .Add(new HttpCookie(SHOPPING_CART_ID_COOKIE_NAME, cartId.ToString()));
        }
    }
}