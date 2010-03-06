namespace NCommon.Data.NHibernate.Tests.HRDomain.Domain
{
    public abstract class Employee
    {
        public virtual int Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual Department Department { get; set; }
        public virtual Employee Manager { get; set; }
        public virtual EmployeeStatus Status { get; set; }
    }
}