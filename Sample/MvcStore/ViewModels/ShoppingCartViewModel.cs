using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MvcStore.Models;
using NCommon.Extensions;

namespace MvcStore.ViewModels
{
    public class ShoppingCartViewModel
    {
        public ShoppingCartViewModel()
        {
            Items = new List<ShoppingCartItemViewModel>();
        }

        public ShoppingCartViewModel(ShoppingCart cart, IEnumerable<string> categories)
        {
            Categories = categories;
            ItemsCount = cart.Items.Count();
            TotalAmount = cart.Total;
            Items = cart.Items.Select(item => new ShoppingCartItemViewModel(item)).ToList();
        }

        public int ItemsCount { get; set; }
        public float TotalAmount { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public List<ShoppingCartItemViewModel> Items { get; set; }
    }
}