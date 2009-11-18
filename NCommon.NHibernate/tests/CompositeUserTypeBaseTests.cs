using System;
using System.Collections.Generic;
using System.Linq;
using NCommon.Extensions;
using NCommon.NHibernate.Tests.Domain;
using NUnit.Framework;

namespace NCommon.Data.NHibernate.Tests
{
    [TestFixture]
    public class CompositeUserTypeBaseTests : NHTestBase
    {
        #region tests
        [Test]
        public void Test_Can_Get_MonthlySalesSummary_With_Money_Type()
        {
            IList<MonthlySalesSummary> report;
            using (var testData = new NHTestDataGenerator(Factory.OpenSession()))
            using (var scope = new UnitOfWorkScope())
            {
                testData.Batch(action => action.CreateMonthlySalesSummaryForMonth(1));

                var repository = new NHRepository<MonthlySalesSummary>();
                report = (from summary in repository
                          where summary.Month == 1
                          select summary).ToList();

                scope.Commit();
            }

            Assert.That(report, Is.Not.Null);
            Assert.That(report.Count, Is.GreaterThan(0));

            report.ForEach(rep =>
                               {
                                   Assert.That(rep.Month == 1);
                                   Assert.That(rep.TotalSale, Is.Not.Null);
                                   Assert.That(rep.TotalSale.Amount, Is.GreaterThan(0));
                                   Assert.That(rep.TotalSale.Currency, Is.Not.Null);
                               });
        } 

        [Test]
        public void Test_Can_Query_MonthlySalesSummary_Based_On_Currency()
        {
            IList<MonthlySalesSummary> report;
            using (var testData = new NHTestDataGenerator(Factory.OpenSession()))
            using (var scope = new UnitOfWorkScope())
            {
                testData.Batch(actions => actions.CreateMonthlySalesSummaryWithAmount(
                    new Money{Amount = 100, Currency = "YEN"}
                    ));

                var repository = new NHRepository<MonthlySalesSummary>();
                report = (from summary in repository
                          where summary.TotalSale.Currency == "YEN"
                          select summary).ToList();

                scope.Commit();
            }

            Assert.That(report, Is.Not.Null);
            Assert.That(report.Count, Is.GreaterThan(0));

            report.ForEach(rep =>
            {
                Assert.That(rep.TotalSale, Is.Not.Null);
                Assert.That(rep.TotalSale.Amount, Is.GreaterThan(0));
                Assert.That(rep.TotalSale.Currency, Is.Not.Null);
                Assert.That(rep.TotalSale.Currency, Is.EqualTo("YEN"));
            });
        }

        [Test]
        public void Test_Updating_Money_Amount_Updates_Amount_In_Store_And_Returns_Updated_Figure ()
        {
            var newAmount = (decimal) new Random().Next();	
            using (var testData = new NHTestDataGenerator(Factory.OpenSession()))
            {
                testData.Batch(actions => actions.CreateMonthlySalesSummaryForSalesPerson(1));

                using (var scope = new UnitOfWorkScope())
                {
                    var repository = new NHRepository<MonthlySalesSummary>();
                    var report = (from summary in repository
                                  where summary.SalesPersonId == 1
                                  select summary).SingleOrDefault();
                    report.TotalSale.Amount = newAmount;
                    scope.Commit();
                }

                using (var scope = new UnitOfWorkScope())
                {
                    var repository = new NHRepository<MonthlySalesSummary>();
                    var report = (from summary in repository
                                  where summary.SalesPersonId == 1
                                  select summary).SingleOrDefault();

                    Assert.That(report.TotalSale.Amount, Is.EqualTo(newAmount));
                    scope.Commit();
                } 
            }
        }
        #endregion
    }
}