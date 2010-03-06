using FluentNHibernate.Mapping;
using NCommon.Data.NHibernate.Tests.HRDomain.Domain;

namespace NCommon.NHibernate.Tests.HRDomain.Mappings
{
    public class SalesTerritoryMap : ClassMap<SalesTerritory>
    {
        public SalesTerritoryMap()
        {
            Id(x => x.Id)
                .GeneratedBy.Identity();
            Map(x => x.Name);
            Map(x => x.Description);
            HasMany(x => x.SalesPersons)
                .AsSet()
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore)
                .KeyColumn("TerritoryId");
        }
    }
}