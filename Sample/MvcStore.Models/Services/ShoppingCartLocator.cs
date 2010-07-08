using System;
using System.Linq;
using System.Web;
using NCommon.Context;
using NCommon.Data;

namespace MvcStoreModels.Services
{
    public class ShoppingCartLocator : IShoppingCartLocator
    {
        readonly IContext _context;
        readonly IRepository<ShoppingCart> _shoppingCartRepository;
        const string SHOPPING_CART_ID_COOKIE_NAME = "MVCStore_ShoppingCartId";

        public ShoppingCartLocator(
            IContext context, 
            IRepository<ShoppingCart> shoppingCartRepository)
        {
            _context = context;
            _shoppingCartRepository = shoppingCartRepository;
        }

        public ShoppingCart GetShoppingCart()
        {
            var cartId = GetCartIdFromCookie();
            ShoppingCart cart = null;
            using (var scope = new UnitOfWorkScope())
            {
                cart = (from carts in _shoppingCartRepository.For<IShoppingCartLocator>()
                        where carts.Id == cartId
                        select carts).SingleOrDefault();
                if (cart == null)
                {
                    cart = ShoppingCart.Create(cartId);
                    _shoppingCartRepository.Save(cart);
                }
                scope.Commit();
            }
            return cart;
        }

        Guid GetCartIdFromCookie()
        {
            var cookie = _context.HttpContext.Request.Cookies[SHOPPING_CART_ID_COOKIE_NAME];
            if (cookie == null)
                return Guid.NewGuid(); //Creating a new cart id.

            Guid cartId;
            return !Guid.TryParse(cookie.Value, out cartId) ? Guid.NewGuid() : cartId;
        }
    }
}