using NCommon.Extensions;

namespace NCommon.LinqToSql.Tests.Domain
{
	partial class Order
	{
		public decimal CalculateTotal()
		{
			decimal total = 0;
			this.OrderItems.ForEach(x => total += x.TotalPrice);
			return total;
		}

		
	}

	public partial class OrderItem
	{
		public decimal TotalPrice
		{
			get { return Price * Quantity; }
		}
	}
}
