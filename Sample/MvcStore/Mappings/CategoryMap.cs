using MvcStore.Models;

namespace MvcStore.Mappings
{
    public class CategoryMap : EntityMap<Category>
    {
        public CategoryMap()
        {
            Map(x => x.Name);
            Map(x => x.Description);
        }
    }
}