using System;

namespace NCommon.NHibernate.Tests.Domain
{
	public class CustomOrderItem
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Store { get; set; }
		public virtual CustomOrder Order { get; set; }
	}
}