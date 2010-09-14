using System;

namespace MvcStore.Models
{
    public class ShoppingCartItem : Entity
    {
        ShoppingCart _shoppingCart;

        protected ShoppingCartItem() {}

        public ShoppingCartItem(ShoppingCart shoppingCart)
        {
            _shoppingCart = shoppingCart;
        }

        public virtual ShoppingCart ShoppingCart
        {
            get { return _shoppingCart; }
        }

        public virtual Guid ProductId { get; set; }
        public virtual string ProductName { get; set; }
        public virtual string ProductCode { get; set; }
        public virtual string ProductCategory { get; set; }
        public virtual float Price { get; set; }
        public virtual int Quantity { get; set; }
        public virtual float Total
        {
            get { return Price*Quantity; }
        }
    }
}