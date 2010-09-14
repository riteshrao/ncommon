using System;
using MvcStore.Models;

namespace MvcStore.ViewModels
{
    public class ShoppingCartItemViewModel
    {
        public ShoppingCartItemViewModel()
        {
        }

        public ShoppingCartItemViewModel(ShoppingCartItem cartItem)
        {
            Id = cartItem.Id;
            Product = new ProductViewModel(cartItem);
            Price = cartItem.Price;
            Quantity = cartItem.Quantity;
            TotalAmount = cartItem.Total;
        }

        public Guid Id { get; set; }
        public ProductViewModel Product { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public float TotalAmount { get; set; }
    }
}