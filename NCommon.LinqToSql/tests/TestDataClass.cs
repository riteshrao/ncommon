using NCommon.Extensions;

namespace NCommon.LinqToSql.Tests
{
	partial class Order
	{
		public decimal CalculateTotal()
		{
			decimal total = 0;
			OrderItems.ForEach(x => total += x.TotalPrice ?? 0);
			return total;
		}
	}

	public partial class OrderItem
	{
		public decimal? TotalPrice
		{
			get { return Price * Quantity; }
		}
	}
}