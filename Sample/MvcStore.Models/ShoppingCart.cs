using System;
using System.Collections.Generic;
using System.Linq;
using NCommon;
using NCommon.Extensions;

namespace MvcStoreModels
{
    public class ShoppingCart
    {
        Guid _id;
        DateTime _created;
        DateTime _lastModified;
        ICollection<ShoppingCartItem> _items;

        protected ShoppingCart() : this(Guid.NewGuid()) {}

        ShoppingCart(Guid cartId)
        {
            _id = cartId;
        }

        public virtual Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual DateTime Created
        {
            get { return _created;}
            set { _created = value; }
        }

        public virtual DateTime LastModified
        {
            get { return _lastModified;}
            set { _lastModified = value; }
        }

        public virtual ICollection<ShoppingCartItem> Items
        {
            get { return _items;}
            set { _items = value; }
        }

        public virtual float Total
        {
            get
            {
                return Items.Sum(x => x.Total);
            }
        }

        public static ShoppingCart Create(Guid cartId)
        {
            return new ShoppingCart(cartId)
            {
                Created = DateTime.Now,
                LastModified = DateTime.Now,
                Items = new HashSet<ShoppingCartItem>()
            };
        }

        public virtual void AddToCart(params Product[] products)
        {
            products.ForEach(AddToCart);
            UdpateLastModified();
        }

        public virtual void AddToCart(Product product)
        {
            Guard.Against<ArgumentNullException>(product == null, "Cannot add a null Product to the cart. " +
                                                                  "The ShoppingCart expects a valid non null Product instance.");
            var existingItem = Items.SingleOrDefault(x => x.ProductId == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
                return;
            }
            var shoppingCartItem = new ShoppingCartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = 1
            };
            shoppingCartItem.SetCart(this);
            _items.Add(shoppingCartItem);
        }

        public virtual void RemoveFromCart(Guid itemId)
        {
            var item = Items.SingleOrDefault(x => x.Id == itemId);
            if (item != null)
                _items.Remove(item);
        }

        public virtual void SetQuantity(Guid itemId, int quantity)
        {
            var item = Items.SingleOrDefault(x => x.Id == itemId);
            if (item == null)
                return;
            if (quantity <= 0)
            {
                _items.Remove(item);
                return;
            }
            item.Quantity = quantity;
        }

        public virtual void ClearCart()
        {
            Items.Clear();
        }

        void UdpateLastModified()
        {
            _lastModified = DateTime.Now;
        }
    }
}