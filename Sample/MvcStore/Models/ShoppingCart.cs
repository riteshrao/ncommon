using System;
using System.Collections.Generic;
using System.Linq;
using NCommon;
using NCommon.Extensions;
using NCommon.Util;

namespace MvcStore.Models
{
    public class ShoppingCart : Entity
    {
        DateTime _created;
        DateTime _lastModified;
        ICollection<ShoppingCartItem> _items;

        public ShoppingCart()
        {
            _created = DateTime.Now;
            _lastModified = DateTime.Now;
            _items = new HashSet<ShoppingCartItem>();
        }

        public virtual DateTime Created
        {
            get { return _created; }
        }

        public virtual DateTime LastModified
        {
            get { return _lastModified; }
        }

        public virtual IEnumerable<ShoppingCartItem> Items
        {
            get {return _items;}
        }

        public virtual float Total
        {
            get { return Items.Sum(x => x.Total); }
        }

        public virtual void AddToCart(params Product[] products)
        {
            if (products.Length == 0)
                return;
            products.ForEach(AddToCart);
        }

        public virtual void AddToCart(Product product)
        {
            Guard.Against<ArgumentNullException>(product == null,
                                                 "Cannot add a null Product to the cart. " +
                                                 "The ShoppingCart expects a valid non-null Product instance.");
            var existingItem = Items.SingleOrDefault(x => x.ProductId == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
                return;
            }
            _items.Add(new ShoppingCartItem(this)
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductCategory = product.Category.Name,
                ProductCode = product.ProductCode,
                Price = product.Price,
                Quantity = 1
            });
            UpdateModified();
        }

        public virtual void RemoveFromCart(Guid itemId)
        {
            var item = Items.SingleOrDefault(x => x.Id == itemId);
            if (item != null)
                _items.Remove(item);
            UpdateModified();
        }

        public virtual void SetQuantity(Guid itemId, int quantity)
        {
            var item = Items.SingleOrDefault(x => x.Id == itemId);
            if (item == null)
                return;
            if (quantity <= 0)
            {
                RemoveFromCart(itemId);
                return;
            }
            item.Quantity = quantity;
            UpdateModified();
        }

        public virtual void ClearCart()
        {
            _items.Clear();
            UpdateModified();
        }

        void UpdateModified()
        {
            _lastModified = DateTime.Now;
        }
    }
}