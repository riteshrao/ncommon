using System.Data.Objects;

namespace MvcStoreModels
{
    public class MvcStoreDataContext : ObjectContext
    {
        readonly ObjectSet<Category> _categories;
        readonly ObjectSet<Product> _products;
        readonly ObjectSet<ShoppingCart> _shoppingCarts;

        public MvcStoreDataContext(string connectionString)
            : base(connectionString, "MvcStoreEntities")
        {
            _categories = CreateObjectSet<Category>();
            _products = CreateObjectSet<Product>();
            _shoppingCarts = CreateObjectSet<ShoppingCart>();
        }

        public ObjectSet<Category> Categories
        {
            get { return _categories; }
        }

        public ObjectSet<Product> Products
        {
            get { return _products; }
        }

        public ObjectSet<ShoppingCart> ShoppingCarts
        {
            get { return _shoppingCarts; }
        }
    }
}