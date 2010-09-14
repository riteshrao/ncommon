namespace MvcStore.Models
{
    public class Product : Entity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string ProductCode { get; set; }
        public virtual float Price { get; set; }
        public virtual Category Category { get; set; }
    }
}