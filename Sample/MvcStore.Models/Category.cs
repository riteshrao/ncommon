using System;
using System.Collections.Generic;

namespace MvcStoreModels
{
    public class Category
    {
        Guid _id;

        public Category()
        {
            _id = Guid.NewGuid();
        }

        public virtual Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }
        public virtual string Description { get; set; }
    }
}