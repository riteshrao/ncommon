using FluentNHibernate.Mapping;

namespace MvcStoreModels.NHibernate.Mappings
{
    public class CategoryMap : ClassMap<Category>
    {
        public CategoryMap()
        {
            Table("Categories");
            Id(x => x.Id)
                .GeneratedBy.Assigned()
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore);
            Map(x => x.Name);
            Map(x => x.Code);
            Map(x => x.Description);
        }
    }
}