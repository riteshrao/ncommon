namespace MvcStore.Models
{
    public class Category : Entity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}