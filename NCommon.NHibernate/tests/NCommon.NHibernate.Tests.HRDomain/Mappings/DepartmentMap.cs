using FluentNHibernate.Mapping;
using NCommon.Data.NHibernate.Tests.HRDomain.Domain;

namespace NCommon.NHibernate.Tests.HRDomain.Mappings
{
    public class DepartmentMap : ClassMap<Department>
    {
        public DepartmentMap()
        {
            Table("Departments");
            Id(x => x.Id)
                .GeneratedBy.Identity();
            Map(x => x.Name);
        }
    }
}