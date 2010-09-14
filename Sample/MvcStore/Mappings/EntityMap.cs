using FluentNHibernate.Mapping;
using MvcStore.Models;
using NCommon.Util;

namespace MvcStore.Mappings
{
    public abstract class EntityMap<T> : ClassMap<T> where T : Entity
    {
        protected EntityMap()
        {
            Table(Inflector.Pluralize(typeof (T).Name));
            Id(x => x.Id)
                .GeneratedBy.GuidComb()
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore);
        }
    }
}