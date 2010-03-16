using System;
using System.Linq;
using System.Transactions;
using NCommon.Data.LinqToSql.Tests.HRDomain;
using NCommon.Data.LinqToSql.Tests.OrdersDomain;
using NUnit.Framework;

namespace NCommon.Data.LinqToSql.Tests
{
    [TestFixture]
    public class LinqToSqlUnitOfWorkTransactionTests : LinqToSqlTestBase
    {
        [Test]
        public void changes_are_persisted_when_ambient_scope_is_committed()
        {
            using (var ordersData = new LinqToSqlDataGenerator(OrdersContextProvider()))
            {
                ordersData.Batch(actions => actions.CreateCustomer());
                using (var ambientScope = new TransactionScope())
                {
                    using (var scope = new UnitOfWorkScope())
                    {
                        var customer = new LinqToSqlRepository<Customer>().First();
                        customer.FirstName = "Changed";
                        scope.Commit();
                    }
                    ambientScope.Complete();
                }

                using (var scope = new UnitOfWorkScope())
                {
                    var customer = new LinqToSqlRepository<Customer>().First();
                    Assert.That(customer.FirstName, Is.EqualTo("Changed"));
                    scope.Commit();
                }
            }
        }

        [Test]
        public void changes_are_not_persisted_when_ambient_transaction_rolls_back()
        {
            using (var ordersData = new LinqToSqlDataGenerator(OrdersContextProvider()))
            {
                ordersData.Batch(actions => actions.CreateCustomer());
                using (new TransactionScope())
                {
                    using (var scope = new UnitOfWorkScope())
                    {
                        var customer = new LinqToSqlRepository<Customer>().First();
                        customer.FirstName = "Changed";
                        scope.Commit();
                    }
                } //Auto rollback

                using (var scope = new UnitOfWorkScope())
                {
                    var customer = new LinqToSqlRepository<Customer>().First();
                    Assert.That(customer.FirstName, Is.Not.EqualTo("Changed"));
                }
            }
        }

        [Test]
        public void when_ambient_transaction_is_running_multiple_scopes_work()
        {
            using (var ordersData = new LinqToSqlDataGenerator(OrdersContextProvider()))
            {
                ordersData.Batch(actions => actions.CreateCustomerInState("LA"));
                using (new TransactionScope())
                {
                    using (var firstUOW = new UnitOfWorkScope())
                    {
                        var repository = new LinqToSqlRepository<Customer>();
                        var query = repository.Where(x => x.State == "LA");
                        Assert.That(query.Count(), Is.GreaterThan(0));
                        firstUOW.Commit();
                    }

                    using (var secondUOW = new UnitOfWorkScope())
                    {
                        var repository = new LinqToSqlRepository<Customer>();
                        repository.Add(new Customer
                        {
                            FirstName = "NHUnitOfWorkTransactionTest",
                            LastName = "Customer",
                            StreetAddress1 = "This recrd was insertd via a test",
                            City = "Fictional City",
                            State = "LA",
                            ZipCode = "00000"
                        });
                        secondUOW.Commit();
                    }
                    //Rolling back changes.
                }
            }
        }

        [Test]
        public void when_ambient_transaction_is_running_and_a_previous_scope_rollsback_new_scope_still_works()
        {
            using (var ordersData = new LinqToSqlDataGenerator(OrdersContextProvider()))
            {
                ordersData.Batch(actions => actions.CreateCustomer());

                string oldCustomerName;
                var newCustomerName = "NewCustomer" + new Random().Next(0, int.MaxValue);
                var newCustomer = new Customer
                {
                    FirstName = newCustomerName,
                    LastName = "Save",
                    StreetAddress1 = "This record was inserted via a test",
                    City = "Fictional City",
                    State = "LA",
                    ZipCode = "00000"
                };

                using (new TransactionScope())
                {
                    using (new UnitOfWorkScope())
                    {
                        var customer = new LinqToSqlRepository<Customer>().First();
                        oldCustomerName = customer.FirstName;
                        customer.FirstName = "Changed";
                    }  //Rollback

                    using (var secondUOW = new UnitOfWorkScope())
                    {
                        new LinqToSqlRepository<Customer>().Add(newCustomer);
                        secondUOW.Commit();
                    }
                }

                using (var scope = new UnitOfWorkScope())
                {
                    var repository = new LinqToSqlRepository<Customer>();
                    Assert.That(repository.First().FirstName, Is.EqualTo(oldCustomerName));
                    Assert.That(repository.Where(x => x.FirstName == newCustomerName).Count(), Is.GreaterThan(0));
                    repository.Attach(newCustomer);
                    repository.Delete(newCustomer);
                    scope.Commit();
                }
            }
        }

     
        [Test]
        public void rolling_back_scope_rollsback_everything_for_all_managed_sessions()
        {
            using (new UnitOfWorkScope())
            {
                var customerRepository = new LinqToSqlRepository<Customer>();
                var salesPersonRepository = new LinqToSqlRepository<Employee>();

                var customer = new Customer
                {
                    FirstName = "Should Not Save",
                    LastName = "Should Not Save."
                };

                var employee = new Employee
                {
                    FirstName = "Should Not Save",
                    LastName = "Should Not Save"
                };

                customerRepository.Save(customer);
                salesPersonRepository.Save(employee);
            } //Rolling back all operations

            using (var scope = new UnitOfWorkScope())
            {
                var customerRepository = new LinqToSqlRepository<Customer>();
                var salesPersonRepository = new LinqToSqlRepository<SalesPerson>();

                var customer = customerRepository.FirstOrDefault();
                var salesPerson = salesPersonRepository.FirstOrDefault();
                Assert.That(customer, Is.Null);
                Assert.That(salesPerson, Is.Null);
                scope.Commit();
            }
        }
    }
}