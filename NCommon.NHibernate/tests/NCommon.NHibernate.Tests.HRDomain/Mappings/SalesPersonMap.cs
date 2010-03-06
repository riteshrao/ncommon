using FluentNHibernate.Mapping;
using NCommon.Data.NHibernate.Tests.HRDomain.Domain;

namespace NCommon.NHibernate.Tests.HRDomain.Mappings
{
    public class SalesPersonMap : SubclassMap<SalesPerson>
    {
        public SalesPersonMap()
        {
            Map(x => x.SalesQuota);
            Map(x => x.SalesYTD);
            References(x => x.Territory)
                .Column("TerritoryId");
        }
    }
}