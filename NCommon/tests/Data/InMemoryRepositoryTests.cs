using System.Collections.Generic;
using System.Linq;
using NCommon.DataServices.Transactions;
using NCommon.ObjectAccess;
using NUnit.Framework;

namespace NCommon.Tests.Data
{
	/// <summary>
	/// Tests the <see cref="InMemoryRepository{TEntity}"/> class.
	/// </summary>
	[TestFixture]
	public class InMemoryRepositoryTests
	{
        [Test]
		public void returns_values_only_contained_in_internal_list ()
		{
			var list = new List<string> {"Apple", "Ball", "Cat", "Dog"};
			var repository = new InMemoryRepository<string>(list);

			var result = from val in repository
			             where val.StartsWith("A")
			             select val;

			Assert.That(result.Count(), Is.EqualTo(1));
			Assert.That(result.First(), Is.EqualTo("Apple"));
		}

        [Test]
		public void Add_adds_to_internal_list()
		{
			var list = new List<string> { "Apple", "Ball", "Cat", "Dog" };
			var repository = new InMemoryRepository<string>(list);

			repository.Add("DoDo");
			Assert.That(list.Contains("DoDo"));
		}

        [Test]
		public void delete_removes_from_internal_list()
		{
			var list = new List<string> { "Apple", "Ball", "Cat", "Dog" };
			var repository = new InMemoryRepository<string>(list);
			repository.Delete("Apple");

			Assert.That(list.Contains("Apple"), Is.False);
		}
	}
}