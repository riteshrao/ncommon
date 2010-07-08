using System;
using NCommon;

namespace MvcStoreModels
{
    public class ShoppingCartItem
    {
        Guid _id;
        ShoppingCart _cart;

        public ShoppingCartItem()
        {
            _id = Guid.NewGuid();
        }

        public virtual Guid Id
        {
            get { return _id; }
        }
        public virtual ShoppingCart Cart
        {
            get { return _cart;}
        }
        public virtual Guid ProductId { get; set; }
        public virtual string ProductName { get; set; }
        public virtual float Price { get; set; }
        public virtual int Quantity { get; set; }
        public virtual float Total
        {
            get { return Price*Quantity; }
        }

        public virtual void SetCart(ShoppingCart cart)
        {
            Guard.Against<ArgumentNullException>(cart == null,
                                                 "Cannot associate a ShoppingCartItem with a null Cart instance.");
            _cart = cart;
        }

        public virtual bool Equals(ShoppingCartItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || other._id.Equals(_id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ShoppingCartItem)) return false;
            return Equals((ShoppingCartItem) obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}