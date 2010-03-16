namespace NCommon.Data.NHibernate.Tests.HRDomain.Domain
{
    public class SalesPerson : Employee
    {
        public SalesPerson()
        {
            base.Type = EmployeeType.SalesPerson;
        }

        public virtual float SalesQuota { get; set; }
        public virtual decimal SalesYTD { get; set;}
        public virtual SalesTerritory Territory { get; set; }
    }
}