using System;
using System.Collections.Generic;

namespace NCommon.NHibernate.Tests.Domain
{
	public class CustomOrder
	{
		public virtual int Id { get; set; }
		public virtual DateTime OrderDate { get; set; }
		public virtual ICollection<CustomOrderItem> Items { get; set; }

	}
}