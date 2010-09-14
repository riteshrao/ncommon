using MvcStore.Models;

namespace MvcStore.Services
{
    public interface IShoppingCartLocator
    {
        ShoppingCart GetCart();
    }
}