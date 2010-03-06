using FluentNHibernate.Mapping;
using NCommon.Data.NHibernate.Tests.HRDomain.Domain;

namespace NCommon.NHibernate.Tests.HRDomain.Mappings
{
    public class SalesManagerMap : SubclassMap<SalesManager>
    {
        public SalesManagerMap()
        {
            HasMany(x => x.Subordinates)
                .AsSet()
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore)
                .KeyColumn("ManagerId");
        }
    }
}