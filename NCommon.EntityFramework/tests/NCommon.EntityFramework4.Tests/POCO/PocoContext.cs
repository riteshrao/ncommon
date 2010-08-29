using System.Data.EntityClient;
using System.Data.Objects;
using NCommon.EntityFramework4.Tests.Models;

namespace NCommon.EntityFramework4.Tests.POCO
{
    public class PocoContext : ObjectContext
    {
        readonly ObjectSet<Customer> _customers;
        readonly ObjectSet<Order> _orders;
        readonly ObjectSet<Product> _products;

        public PocoContext(string connectionString) : base(connectionString)
        {
            DefaultContainerName = "Entities";
            _customers = CreateObjectSet<Customer>("Customers");
            _orders = CreateObjectSet<Order>("Orders");
            _products = CreateObjectSet<Product>("Products");
            ContextOptions.LazyLoadingEnabled = true;
        }

        public ObjectSet<Customer> Customers
        {
            get { return _customers; }
        }

        public ObjectSet<Order> Orders
        {
            get { return _orders; }
        }

        public ObjectSet<Product> Products
        {
            get { return _products; }
        }
    }
}