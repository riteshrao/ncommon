using System;
using Db4objects.Db4o;
using NCommon.Db4o.Tests.Domain;

namespace NCommon.Db4o.Tests
{
    public class Db4oTestDataActions
    {
        readonly Db4oTestDataGenerator _generator;
        readonly Random _random = new Random();

        public Db4oTestDataActions(Db4oTestDataGenerator generator)
        {
            _generator = generator;
        }

        public Customer CreateCustomer()
        {
            var customer = new Customer
            {
                FirstName = "John" + RandomString(),
                LastName = "Doe" + RandomString()
            };
            _generator.Container.Store(customer);
            _generator.EntitiesPersisted.Add(customer);
            return customer;
        }

        public Address CreateAddress()
        {
            return new Address
            {
                StreetAddress1 = "123 Main St " + RandomString(),
                StreetAddress2 = "4th Floor " + RandomString(),
                City = "Sunshine Valley",
                State = "NY",
                ZipCode = "10001"
            };
        }

        public Customer CreateCustomerInState(string state)
        {
            var customer = CreateCustomer();
            customer.Address = CreateAddress();
            customer.Address.State = state;
            return customer;
        }

        public Customer[] CreateCustomersInState(string state, int count)
        {
            var customers = new Customer[count];
            for (var i = 0; i < count; i++)
                customers[i] = CreateCustomerInState(state);
            return customers;
        }

        public Order CreateOrderForCustomer(Customer customer)
        {
            var order = new Order
            {
                Customer = customer,
                OrderDate = DateTime.Now.AddDays(-5),
                ShipDate = DateTime.Now.AddDays(5)
            };
            _generator.Container.Store(order);
            _generator.EntitiesPersisted.Add(order);
            return order;
        }

        public Order CreateOrderForProducts(Product[] products)
        {
            var order = CreateOrderForCustomer(CreateCustomer());
            foreach (var product in products)
                order.Items.Add(CreateItem(order, product));
            return order;
        }

        public Order[] CreateOrdersForCustomers(params Customer[] customers)
        {
            var orders = new Order[customers.Length];
            for (var i = 0; i < customers.Length; i++)
                orders[i] = CreateOrderForCustomer(customers[i]);
            return orders;
        }

        public Product CreateProduct()
        {
            var product = new Product
            {
                Name = "Product" + RandomString(),
                Description = "Product Description" + RandomString()
            };
            _generator.Container.Store(product);
            _generator.EntitiesPersisted.Add(product);
            return product;
        }

        public Product[] CreateProducts(int count)
        {
            var products = new Product[count];
            for (var i = 0; i < count; i++)
                products[i] = CreateProduct();
            return products;
        }

        public OrderItem CreateItem(Order order, Product product)
        {
            return new OrderItem
            {
                Order = order,
                Price = 1,
                Product = product,
                Quantity = 3,
                Store = "Internet"
            };
        }

        protected string RandomString()
        {
            return _random.Next(int.MaxValue).ToString();
        }
    }
}