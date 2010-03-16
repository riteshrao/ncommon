using FluentNHibernate.Mapping;
using NCommon.Data.NHibernate.Tests.HRDomain.Domain;

namespace NCommon.NHibernate.Tests.HRDomain.Mappings
{
    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Table("Employees");
            Id(x => x.Id)
                .GeneratedBy.Identity();
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Type);
            References(x => x.Department)
                .Column("DepartmentId");
            References(x => x.Manager)
                .Column("ManagerId");
        }
    }
}