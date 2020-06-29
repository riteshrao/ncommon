using System.Linq;

namespace NCommon.LinqToSql.Tests.OrdersDomain
{
    partial class Order
    {
        public decimal? CalculateTotal()
        {
            return OrderItems.Sum(x => x.Price*x.Quantity);
        }
    }
}
