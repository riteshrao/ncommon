using System.Collections.Generic;
using MvcStore.Models;

namespace MvcStore.ViewModels
{
    public class StoreViewModel
    {
        public string CurrentCategory { get; set; }
        public IList<string> Categories { get; set; }
        public IList<ProductSummary> Products { get; set; }
    }
}