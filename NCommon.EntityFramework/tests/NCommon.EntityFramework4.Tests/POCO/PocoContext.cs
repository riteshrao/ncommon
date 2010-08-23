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
            _customers = CreateObjectSet<Customer>();
            _orders = CreateObjectSet<Order>();
            _products = CreateObjectSet<Product>();
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