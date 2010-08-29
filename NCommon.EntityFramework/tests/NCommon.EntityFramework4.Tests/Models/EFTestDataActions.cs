using System;

namespace NCommon.EntityFramework4.Tests.Models
{
    public class EFTestDataActions
    {
        readonly EFTestData _generator;
        readonly Random _random = new Random();

        public EFTestDataActions(EFTestData generator)
        {
            _generator = generator;
        }

        public Customer CreateCustomer()
        {
            return CreateCustomer(x => { });
        }

        public Customer CreateCustomer(Action<Customer> customize)
        {
            var customer = _generator.Context.CreateObject<Customer>();
            customer.FirstName = "John" + RandomString();
            customer.LastName = "Doe" + RandomString();
            customer.StreetAddress1 = "123 Main St " + RandomString();
            customer.StreetAddress2 = "4th Floor " + RandomString();
            customer.City = "Sunshine Valley";
            customer.State = "CA";
            customer.ZipCode = "10001";
            customize(customer);
            _generator.Context.AddObject("Customers", customer);
            return customer;
        }

        public Order CreateOrder()
        {
            return CreateOrder(x => { });
        }

        public Order CreateOrderForCustomer(Customer customer)
        {
            var order = CreateOrder(x => x.Customer = customer);
            customer.Orders.Add(order);
            return order;
        }

        public Order CreateOrder(Action<Order> customize)
        {
            var order = new Order
            {
                OrderDate = DateTime.Now,
                ShipDate = DateTime.Now.AddDays(10)
            };
            order.OrderItems.Add(CreateOrderItem(x => x.Order = order));
            order.OrderItems.Add(CreateOrderItem(x => x.Order = order));
            customize(order);
            _generator.Context.AddObject("Orders", order);
            return order;
        }

        public OrderItem CreateOrderItem(Action<OrderItem> customize)
        {
            var orderItem = _generator.Context.CreateObject<OrderItem>();
            orderItem.Product = CreateProduct(x => { });
            orderItem.Price = 10;
            orderItem.Quantity = 100;
            orderItem.Store = RandomString();
            customize(orderItem);
            _generator.Context.AddObject("OrderItems", orderItem);
            return orderItem;
        }

        public Product CreateProduct(Action<Product> customtize)
        {
            var product = _generator.Context.CreateObject<Product>();
            product.Name = RandomString();
            product.Description = RandomString();
            customtize(product);
            _generator.Context.AddObject("Products", product);
            return product;
        }

        string RandomString()
        {
            return _random.Next(int.MaxValue).ToString();
        }
    }
}