using System;
using System.Linq;
using System.Transactions;
using NCommon.Data.EntityFramework.Tests.HRDomain;
using NCommon.Data.EntityFramework.Tests.OrdersDomain;
using NUnit.Framework;

namespace NCommon.Data.EntityFramework.Tests
{
    public class EFUnitOfWorkTransactionTests : EFTestBase
    {
        [Test]
        public void changes_are_persisted_when_ambient_scope_is_committed()
        {
            using (var testData = new EFDataGenerator(OrdersContextProvider()))
            {
                testData.Batch(actions => actions.CreateCustomer());
                using (var ambientScope = new TransactionScope())
                {
                    using (var scope = new UnitOfWorkScope())
                    {
                        var customer = new EFRepository<Customer>().First();
                        customer.FirstName = "Changed";
                        scope.Commit();
                    }
                    ambientScope.Complete();
                }

                using (var scope = new UnitOfWorkScope())
                {
                    var customer = new EFRepository<Customer>().First();
                    Assert.That(customer.FirstName, Is.EqualTo("Changed"));
                    scope.Commit();
                }
            }
        }

        [Test]
        public void changes_are_not_persisted_when_ambient_transaction_rolls_back()
        {
            using (var testData = new EFDataGenerator(OrdersContextProvider()))
            {
                testData.Batch(actions => actions.CreateCustomer());
                using (var ambientScope = new TransactionScope())
                {
                    using (var scope = new UnitOfWorkScope())
                    {
                        var customer = new EFRepository<Customer>().First();
                        customer.FirstName = "Changed";
                        scope.Commit();
                    }
                } //Auto rollback

                using (var scope = new UnitOfWorkScope())
                {
                    var customer = new EFRepository<Customer>().First();
                    Assert.That(customer.FirstName, Is.Not.EqualTo("Changed"));
                }
            }
        }

        [Test]
        public void when_ambient_transaction_is_running_multiple_scopes_work()
        {
            using (var testData = new EFDataGenerator(OrdersContextProvider()))
            {
                testData.Batch(actions => actions.CreateCustomerInState("LA"));
                using (var ambientScope = new TransactionScope())
                {
                    using (var firstUOW = new UnitOfWorkScope())
                    {
                        var repository = new EFRepository<Customer>();
                        var query = repository.Where(x => x.State == "LA");
                        Assert.That(query.Count(), Is.GreaterThan(0));
                        firstUOW.Commit();
                    }

                    using (var secondUOW = new UnitOfWorkScope())
                    {
                        var repository = new EFRepository<Customer>();
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
            using (var testData = new EFDataGenerator(OrdersContextProvider()))
            {
                Customer customer = null;
                testData.Batch(actions => customer = actions.CreateCustomer());

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

                using (var ambientScope = new TransactionScope())
                {
                    using (var firstUOW = new UnitOfWorkScope())
                    {
                        var oldCustomer = new EFRepository<Customer>().Where(x => x.CustomerID == customer.CustomerID).First();
                        oldCustomerName = oldCustomer.FirstName;
                        oldCustomer.FirstName = "Changed";
                    }  //Rollback

                    using (var secondUOW = new UnitOfWorkScope())
                    {
                        var repository = new EFRepository<Customer>();
                        repository.Add(newCustomer);
                        secondUOW.Commit();
                    }
                }

                using (var scope = new UnitOfWorkScope())
                {
                    var repository = new EFRepository<Customer>();
                    var oldCustomer = repository.Where(x => x.CustomerID == customer.CustomerID).First();
                    var addedCustomer = repository.Where(x => x.CustomerID == newCustomer.CustomerID).FirstOrDefault();
                    Assert.That(oldCustomer.FirstName, Is.EqualTo(oldCustomerName));
                    Assert.That(addedCustomer, Is.Not.Null);
                    repository.Delete(addedCustomer);
                    scope.Commit();
                }
            }
        }

        [Test]
        public void rolling_back_scope_rollsback_everything_for_all_managed_sessions()
        {
            using (new UnitOfWorkScope())
            {
                var customerRepository = new EFRepository<Customer>();
                var salesPersonRepository = new EFRepository<SalesTerritory>();

                var customer = new Customer
                {
                    FirstName = "Should Not Save",
                    LastName = "Should Not Save."
                };

                var salesPerson = new SalesTerritory()
                {
                    Description = "Should Not Save",
                    Name = "Should Not Save"
                };

                customerRepository.Save(customer);
                salesPersonRepository.Save(salesPerson);
            } //Rolling back all operations

            using (var scope = new UnitOfWorkScope())
            {
                var customerRepository = new EFRepository<Customer>();
                var salesTerritories = new EFRepository<SalesTerritory>();

                var customer = customerRepository.FirstOrDefault();
                var salesPerson = salesTerritories.FirstOrDefault();
                Assert.That(customer, Is.Null);
                Assert.That(salesPerson, Is.Null);
                scope.Commit();
            }
        }
    }
}