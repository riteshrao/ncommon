using NCommon.Extensions;

namespace NCommon.Data.EntityFramework.Tests
{
    public partial class OrderItem
    {
        public decimal TotalPrice
        {
            get { return Price*Quantity;}
        }
    }

    public partial class Order
    {
        public decimal CalculateTotal ()
        {
            decimal total = 0;
            if (!OrderItems.IsLoaded)
                OrderItems.Load();
            OrderItems.ForEach(x => total += x.TotalPrice);
            return total;
        }
    }
}