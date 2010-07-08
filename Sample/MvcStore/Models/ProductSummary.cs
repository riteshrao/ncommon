using System;

namespace MvcStore.Models
{
    public class ProductSummary
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}