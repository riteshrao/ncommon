#region license
//Copyright 2008 Ritesh Rao 

//Licensed under the Apache License, Version 2.0 (the "License"); 
//you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 

//http://www.apache.org/licenses/LICENSE-2.0 

//Unless required by applicable law or agreed to in writing, software 
//distributed under the License is distributed on an "AS IS" BASIS, 
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and 
//limitations under the License. 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Microsoft.Practices.ServiceLocation;
using NCommon.Data.NHibernate.Tests.Domain;
using NCommon.Extensions;
using NCommon.Specifications;
using NHibernate;
using NUnit.Framework;
using Rhino.Mocks;
using IsolationLevel=System.Data.IsolationLevel;

namespace NCommon.Data.NHibernate.Tests
{
	/// <summary>
	/// Tests the <see cref="NHRepository{TEntity}"/> class.
	/// </summary>
	[TestFixture]
	public class NHRepositoryTests : NHTestBase
	{
		class Order_OrderItems_Strategy : IFetchingStrategy<Order, NHRepositoryTests>
		{
			#region Implementation of IFetchingStrategy<Order,NHUnitOfWorkTests>
			///<summary>
			/// Instructs the instance to define the fetching strategy on the repository instance.
			///</summary>
			///<param name="repository"></param>
			public void Define(IRepository<Order> repository)
			{
				repository.With(x => x.Items);
			}
			#endregion
		}

		public class OrderItems_Product_Strategy : IFetchingStrategy<Order, NHRepositoryTests>
		{
			#region Implementation of IFetchingStrategy<Order,NHUnitOfWorkTests>
			///<summary>
			/// Instructs the instance to define the fetching strategy on the repository instance.
			///</summary>
			///<param name="repository"></param>
			public void Define(IRepository<Order> repository)
			{
				repository.With<OrderItem>(x => x.Product);
			}
			#endregion
		}

		[Test]
		public void Delete_Deletes_Record()
		{
			//Adding a dummy record.
			var newAddress = new Address
			{
				StreetAddress1 = "This record was inserted for deletion",
				City = "Fictional city",
				State = "LA",
				ZipCode = "12345"
			};

			var newCustomer = new Customer
			{
				FirstName = ("John_DELETE_ME_" + DateTime.Now),
				LastName = ("Doe_DELETE_ME_" + DateTime.Now),
				Address = newAddress
			};

			//Re-usable query to query for the matching record.
			var queryForCustomer = new Func<NHRepository<Customer>, Customer>
				(
				x => (from cust in x
				      where cust.FirstName == newCustomer.FirstName && cust.LastName == newCustomer.LastName
				      select cust).FirstOrDefault()
				);
            
			using (var scope = new UnitOfWorkScope())
			{
				var customerRepository = new NHRepository<Customer>();
				var recordCheckResult = queryForCustomer(customerRepository);
				Assert.That(recordCheckResult, Is.Null);

				customerRepository.Add(newCustomer);
				scope.Commit();
			}

			//Retrieve the record for deletion.
			using (var scope = new UnitOfWorkScope())
			{
				var customerRepository = new NHRepository<Customer>();
				var customerToDelete = queryForCustomer(customerRepository);
				Assert.That(customerToDelete, Is.Not.Null);
				customerRepository.Delete(customerToDelete);
				scope.Commit();
			}

			//Ensure customer record is deleted.
			using (new UnitOfWorkScope())
			{
				var customerRepository = new NHRepository<Customer>();
				var recordCheckResult = queryForCustomer(customerRepository);
				Assert.That(recordCheckResult, Is.Null);
			}
		}

		[Test]
		public void Nested_UnitOfWork_With_Different_Transaction_Compatibility_Works()
		{
			int orderId;
			var changedShipDate = DateTime.Now.AddDays(1);
			var changedOrderDate = DateTime.Now.AddDays(2);
			using (new UnitOfWorkScope())
			{
				var ordersRepository = new NHRepository<Order>();
				orderId = ordersRepository.Select(x => x.OrderID).First();
			}

			Assert.NotNull(orderId);
			using (new UnitOfWorkScope())
			{
				var outerRepository = new NHRepository<Order>();
				var outerOrder = outerRepository.Where(x => x.OrderID == orderId).First();
				outerOrder.OrderDate = changedOrderDate;

				using (var innerScope = new UnitOfWorkScope(UnitOfWorkScopeTransactionOptions.CreateNew))
				{
					var innerRepository = new NHRepository<Order>();
					var innerOrder = innerRepository.Where(x => x.OrderID == orderId).First();
					innerOrder.ShipDate = changedShipDate;
					innerScope.Commit();
				}
			}

			using (new UnitOfWorkScope())
			{
				var ordersRepository = new NHRepository<Order>();
				var order = ordersRepository.First();
				Assert.That(order.OrderDate, Is.Not.EqualTo(changedOrderDate));
				Assert.That(order.ShipDate, Is.Not.EqualTo(changedShipDate));
			}
		}

		[Test]
		public void Query_Allows_Eger_Loading_Using_With()
		{
			List<Order> results;
			using (new UnitOfWorkScope())
			{
				var ordersRepository = new NHRepository<Order>();
				results = (from order in ordersRepository.With(x => x.Customer)
				           select order).ToList();
			}
			Assert.DoesNotThrow(() => results.ForEach(x =>
			{
				Assert.That(x.Customer, Is.TypeOf(typeof (Customer)));
				Assert.That(!string.IsNullOrEmpty(x.Customer.FirstName));
			}));
		}

		[Test]
		public void Query_Allows_Lazy_Load_While_UnitOfWork_Still_Running()
		{
			using (new UnitOfWorkScope())
			{
				var ordersRepository = new NHRepository<Order>();
				var results = from order in ordersRepository
				              select order;

				Assert.DoesNotThrow(() => results.ForEach(x =>
				{
					Assert.That(x.Customer, Is.Not.TypeOf(typeof (Customer)));
					Assert.That(!string.IsNullOrEmpty(x.Customer.FirstName));
				}));
			}
		}

		[Test]
		public void Query_Allows_Projection_Using_Select_Projection()
		{
			using (new UnitOfWorkScope())
			{
				var ordersRepository = new NHRepository<Order>();
				var results = from order in ordersRepository
				              select new {order.Customer.FirstName, order.Customer.LastName, order.ShipDate, order.OrderDate};

				Assert.DoesNotThrow(() => results.ForEach(x =>
				{
					Assert.That(!string.IsNullOrEmpty(x.LastName));
					Assert.That(!string.IsNullOrEmpty(x.FirstName));
				}));
			}
		}

		[Test]
		public void Query_Throws_Exception_When_LazyLoading_After_UnitOfWork_Is_Finished()
		{
			Customer customer;
			using (new UnitOfWorkScope())
			{
				var customerRepository = new NHRepository<Customer>();
				customer = (from cust in customerRepository select cust).FirstOrDefault();
			}
			Assert.That(customer, Is.Not.Null);
			Assert.Throws<LazyInitializationException>(() => customer.Orders.Count());
		}

		[Test]
		public void Query_Using_OrderBy_With_QueryMethod_Returns_Matched_Records_Only()
		{
			using (new UnitOfWorkScope())
			{
				var customersInPA = new Specification<Order>(x => x.Customer.Address.State == "PA");

				var ordersRepository = new NHRepository<Order>();
				var results = from order in ordersRepository.Query(customersInPA)
				              orderby order.OrderDate
				              select order;

				Assert.That(results.Count() > 0);
			}
		}

		[Test]
		public void Query_Using_QueryMethod_Returns_Matched_Records_Only()
		{
			using (new UnitOfWorkScope())
			{
				var customersInPA = new Specification<Order>(x => x.Customer.Address.State == "PA");

				var ordersRepository = new NHRepository<Order>();
				var results = from order in ordersRepository.Query(customersInPA) select order;

				Assert.That(results.Count() > 0);
			}
		}

		[Test]
		public void Query_Using_Specifications_With_Closure_Works()
		{
			var state = string.Empty;
			var statesToTestFor = new[] {"PA", "CA"};
			var spec = new Specification<Order>((order) => order.Customer.Address.State == state);
			var repository = new NHRepository<Order>();


			using (new UnitOfWorkScope())
			{
				var query = from order in repository.Query(spec)
				            select order;

				statesToTestFor.ForEach
					(
					testState =>
					{
						state = testState;
						var results = query.ToArray();
						results.ForEach
							(
							result => Assert.That(result.Customer.Address.State, Is.EqualTo(testState))
							);
					}
					);
			}
		}

		[Test]
		public void Query_With_Incompatible_UnitOfWork_Throws_InvalidOperationException()
		{
			var mockUnitOfWork = MockRepository.GenerateStub<IUnitOfWork>();
			UnitOfWork.Current = mockUnitOfWork;
			Assert.Throws<InvalidOperationException>(() =>
			{
				var customerRepository = new NHRepository<Customer>();
				var results = from customer in customerRepository
				              select customer;
			}
				);
			UnitOfWork.Current = null;
		}

		[Test]
		public void Query_With_No_UnitOfWork_Throws_InvalidOperationException()
		{
			Assert.Throws<InvalidOperationException>(() => { new NHRepository<Customer> {new Customer()}; });
		}

		[Test]
		public void Repository_For_Uses_Registered_Fetching_Strategies()
		{
			IEnumerable<Order> orders;
			using (new UnitOfWorkScope())
			{
				var strategies = new IFetchingStrategy<Order, NHRepositoryTests>[]
				{
					new Order_OrderItems_Strategy(),
					new OrderItems_Product_Strategy()
				};

				IRepository<Order> ordersRepository = null;
				ServiceLocator.Current.Expect(x => x.GetAllInstances<IFetchingStrategy<Order, NHRepositoryTests>>())
					.Return(strategies);

				ordersRepository = new NHRepository<Order>().For<NHRepositoryTests>();
				orders = (from o in ordersRepository select o).ToList();
			}
			orders.ForEach(x => Assert.That(x.CalculateTotal(), Is.GreaterThan(0)));
		}

		[Test]
		public void Save_Does_Not_Save_New_Customer_When_UnitOfWork_Is_Aborted()
		{
			var rnd = new Random();
			var newAddress = new Address
			{
				StreetAddress1 = "This record was inserted via a test",
				City = "Fictional city",
				State = "LA",
				ZipCode = "12345"
			};

			var newCustomer = new Customer
			{
				FirstName = ("John_" + rnd.Next(60000, 80000)),
				LastName = ("Doe_" + rnd.Next(60000, 80000)),
				Address = newAddress
			};

			using (new UnitOfWorkScope())
			{
				var customerRepository = new NHRepository<Customer>();
				var recordCheckResult = (from cust in customerRepository
				                         where cust.FirstName == newCustomer.FirstName &&
				                               cust.LastName == newCustomer.LastName
				                         select cust).FirstOrDefault();
				Assert.That(recordCheckResult, Is.Null);

				customerRepository.Add(newCustomer);
				//DO NOT CALL COMMIT TO SIMMULATE A ROLLBACK.
			}

			//Starting a completely new unit of work and repository to check for existance.
			using (var scope = new UnitOfWorkScope())
			{
				var customerRepository = new NHRepository<Customer>();
				var recordCheckResult = (from cust in customerRepository
				                         where cust.FirstName == newCustomer.FirstName &&
				                               cust.LastName == newCustomer.LastName
				                         select cust).FirstOrDefault();
				Assert.That(recordCheckResult, Is.Null);
				scope.Commit();
			}
		}

		[Test]
		public void Save_New_Customer_Saves_Customer_When_UnitOfWork_Is_Committed()
		{
			var newAddress = new Address
			{
				StreetAddress1 = "This record was inserted via a test",
				City = "Fictional city",
				State = "LA",
				ZipCode = "12345"
			};

			var newCustomer = new Customer
			{
				FirstName = ("John_" + DateTime.Now),
				LastName = ("Doe_" + DateTime.Now),
				Address = newAddress
			};

			var queryForCustomer = new Func<NHRepository<Customer>, Customer>
				(
				x => (from cust in x
				      where cust.FirstName == newCustomer.FirstName && cust.LastName == newCustomer.LastName
				      select cust).FirstOrDefault()
				);

			using (var scope = new UnitOfWorkScope())
			{
				var customerRepository = new NHRepository<Customer>();
				var recordCheckResult = queryForCustomer(customerRepository);
				Assert.That(recordCheckResult, Is.Null);

				customerRepository.Add(newCustomer);
				scope.Commit();
			}

			//Starting a completely new unit of work and repository to check for existance.
			using (var scope = new UnitOfWorkScope())
			{
				var customerRepository = new NHRepository<Customer>();
				var recordCheckResult = queryForCustomer(customerRepository);
				Assert.That(recordCheckResult, Is.Not.Null);
				Assert.That(recordCheckResult.FirstName, Is.EqualTo(newCustomer.FirstName));
				Assert.That(recordCheckResult.LastName, Is.EqualTo(newCustomer.LastName));
				scope.Commit();
			}
		}

		[Test]
		public void Save_Updates_Existing_Order_Record()
		{
			int orderIDRetrieved;
			var updatedDate = DateTime.Now;
			var randomOrderNo = new Random();

			using (var scope = new UnitOfWorkScope())
			{
				var orderRepository = new NHRepository<Order>();
				var order = (from o in orderRepository
				             where o.OrderID == randomOrderNo.Next(1, 3)
				             select o).FirstOrDefault();

				Assert.That(order, Is.Not.Null);

				orderIDRetrieved = order.OrderID;
				order.OrderDate = updatedDate;

				scope.Commit();
			}

			using (new UnitOfWorkScope())
			{
				var orderRepository = new NHRepository<Order>();
				var order = (from o in orderRepository
				             where o.OrderID == orderIDRetrieved
				             select o).FirstOrDefault();

				Assert.That(order, Is.Not.Null);
				Assert.That(order.OrderDate.Date, Is.EqualTo(updatedDate.Date));
				Assert.That(order.OrderDate.Hour, Is.EqualTo(updatedDate.Hour));
				Assert.That(order.OrderDate.Minute, Is.EqualTo(updatedDate.Minute));
				Assert.That(order.OrderDate.Second, Is.EqualTo(updatedDate.Second));
			}
		}

		[Test]
		public void testing_specification_with_sub_query()
		{
			using (var scope = new UnitOfWorkScope())
			{
				var result = new NHRepository<Order>()
					.Query(new Specification<Order>(order =>
					                                order.Items.Any(item => item.Store == "StoreC")))
					.Where(order => order.OrderID == 1).FirstOrDefault();

				Assert.That(result, Is.Not.Null);
				Assert.That(result.Items.Count(), Is.GreaterThan(0));
			}
		}

		[Test]
		public void UnitOfWork_is_rolledback_when_containing_TransactionScope_is_rolledback()
		{
			int orderId;
			DateTime oldDate;

			using (var txScope = new TransactionScope(TransactionScopeOption.Required))
			using (var uowScope = new UnitOfWorkScope(IsolationLevel.Serializable))
			{
				var ordersRepository = new NHRepository<Order>();
				var order = (from o in ordersRepository
				             select o).First();

				oldDate = order.OrderDate;
				order.OrderDate = DateTime.Now;
				orderId = order.OrderID;
				uowScope.Commit();
				//Note: txScope has not been committed
			}

			using (var uowScope = new UnitOfWorkScope())
			{
				var ordersRepository = new NHRepository<Order>();
				var order = (from o in ordersRepository
				             where o.OrderID == orderId
				             select o).First();

				Assert.That(order.OrderDate, Is.EqualTo(oldDate));
			}
		}

		[Test]
		public void When_Calling_CalculateTotal_On_Order_Returns_Valid_When_Under_UnitOfWork()
		{
			using (new UnitOfWorkScope())
			{
				var oredersRepository = new NHRepository<Order>();
				var order = (from o in oredersRepository
				             select o).FirstOrDefault();

				Assert.That(order.CalculateTotal(), Is.GreaterThan(0));
			}
		}

		[Test]
		public void When_Calling_CalculateTotal_On_Order_Returns_Valid_With_No_UnitOfWork_Throws()
		{
			Order order;
			using (new UnitOfWorkScope())
			{
				var oredersRepository = new NHRepository<Order>();
				order = (from o in oredersRepository
				         select o).FirstOrDefault();
			}
			Assert.Throws<LazyInitializationException>(() => order.CalculateTotal());
		}

		[Test]
		public void When_No_FetchingStrategy_Registered_For_Makes_No_Changes()
		{
			Order order;
			using (new UnitOfWorkScope())
			{
				var oredersRepository = new NHRepository<Order>().For<NHRepositoryTests>();
				order = (from o in oredersRepository
				         select o).FirstOrDefault();
			}
			Assert.Throws<LazyInitializationException>(() => order.CalculateTotal());
		}

		[Test]
		public void Verify_delete_works ()
		{
			var customerA = new Customer
			{
				FirstName = "CustomerA",
				LastName = "CustomerB",
				Address = new Address
				{
					StreetAddress1 = "Street A",
					StreetAddress2 = "Street B",
					City = "City",
					State = "State"
				}
			};

			var customerB = new Customer
			{
				FirstName = "CustomerA",
				LastName = "CustomerB",
				Address = new Address
				{
					StreetAddress1 = "Street A",
					StreetAddress2 = "Street B",
					City = "City",
					State = "State"
				}
			};

			var repository = new NHRepository<Customer>(Factory.OpenSession());
			var customerCount = 0;
			using (var scope = new UnitOfWorkScope())
			{
				customerCount = repository.Count();
			}

			using (var scope = new UnitOfWorkScope(IsolationLevel.ReadCommitted, UnitOfWorkScopeTransactionOptions.CreateNew))
			{
				repository.Add(customerA);
				repository.Add(customerB);
				scope.Commit();
			}

			using (var scope = new UnitOfWorkScope())
			{
				Assert.That(repository.Count(), Is.EqualTo(customerCount + 2));
				scope.Commit();
			}
			
			using (var scope = new UnitOfWorkScope())
			{
				repository.Delete(customerB);
				scope.Commit();
			}

			using (var scope = new UnitOfWorkScope())
			{
				Assert.That(repository.Count(), Is.EqualTo(customerCount + 1));
				scope.Commit();
			}
		}
	}
}