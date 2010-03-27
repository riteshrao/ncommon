using FluentNHibernate.Mapping;
using NCommon.Data.NHibernate.Tests.HRDomain.Domain;

namespace NCommon.NHibernate.Tests.HRDomain.Mappings
{
    public class SalesPersonMap : ClassMap<SalesPerson>
    {
        public SalesPersonMap()
        {
            Id(x => x.Id)
                .GeneratedBy.Identity();
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.SalesQuota);
            Map(x => x.SalesYTD);
            References(x => x.Department)
                .Column("DepartmentId");
            References(x => x.Territory)
                .Column("TerritoryId");
        }
    }
}