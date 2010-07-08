using System;

namespace MvcStoreModels
{
    public class Product
    {
        Guid _id;

        public Product()
        {
            _id = Guid.NewGuid();
        }

        public virtual Guid Id
        {
            get { return _id; }
            set { _id = value;}
        }
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }
        public virtual float Price { get; set; }
        public virtual Guid CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}