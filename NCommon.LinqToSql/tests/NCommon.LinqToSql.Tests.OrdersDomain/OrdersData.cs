using System.Linq;

namespace NCommon.Data.LinqToSql.Tests.OrdersDomain
{
    partial class Order
    {
        public decimal? CalculateTotal()
        {
            return OrderItems.Sum(x => x.Price*x.Quantity);
        }
    }
}
