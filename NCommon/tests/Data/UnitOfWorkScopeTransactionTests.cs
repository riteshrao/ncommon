using System;
using System.Data;
using NCommon.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Tests.Data
{
	[TestFixture]
	public class UnitOfWorkScopeTransactionTests
	{
		[Test]
		public void GetTransactionForScope_throws_InvalidOperation_Exception_with_UseCompatible_and_CreateNew_Options()
		{
			var options = UnitOfWorkScopeTransactionOptions.UseCompatible | UnitOfWorkScopeTransactionOptions.CreateNew;
			Assert.Throws<InvalidOperationException>(() => UnitOfWorkScopeTransaction.GetTransactionForScope(null, IsolationLevel.Serializable, options));
		}
	}
}