using System.Linq;

namespace NCommon.Data.EntityFramework.Tests.OrdersDomain
{
    public partial class Order
    {
        public decimal? CalculateTotal()
        {
            OrderItems.Load();
            return OrderItems.Sum(x => x.Price*x.Quantity);
        }
    }
}