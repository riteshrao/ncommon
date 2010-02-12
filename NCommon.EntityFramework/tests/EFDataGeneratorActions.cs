using System;

namespace NCommon.Data.EntityFramework.Tests
{
    public class EFDataGeneratorActions
    {
        readonly EFDataGenerator _generator;
        readonly Random _random = new Random();

        public EFDataGeneratorActions(EFDataGenerator generator)
        {
            _generator = generator;
        }

        public Customer CreateCustomer()
        {
            var customer = new Customer
            {
                FirstName = "John" + RandomString(),
                LastName = "Doe" + RandomString(),
                StreetAddress1 = "123 Main St " + RandomString(),
                StreetAddress2 = "4th Floor " + RandomString(),
                City = "Sunshine Valley",
                State = "NY",
                ZipCode = "10001"
            };
            _generator.Context.AddToCustomerSet(customer);
            _generator.EntityDeleteActions.Add(context => context.DeleteObject(customer));
            return customer;
        }

        public Customer CreateCustomerInState(string state)
        {
            var customer = new Customer
            {
                FirstName = "John" + RandomString(),
                LastName = "Doe" + RandomString(),
                StreetAddress1 = "123 Main St " + RandomString(),
                StreetAddress2 = "4th Floor " + RandomString(),
                City = "Sunshine Valley",
                State = state,
                ZipCode = "10001"
            };
            _generator.Context.AddToCustomerSet(customer);
            _generator.EntityDeleteActions.Add(context => context.DeleteObject(customer));
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
                Customers = customer,
                OrderDate = DateTime.Now.AddDays(-5),
                ShipDate = DateTime.Now.AddDays(5)
            };
            _generator.Context.AddToOrderSet(order);
            _generator.EntityDeleteActions.Add(context => context.DeleteObject(order));
            return order;
        }

        public Order CreateOrderForProducts(Product[] products)
        {
            var order = CreateOrderForCustomer(CreateCustomer());
            foreach (var product in products)
                order.OrderItems.Add(CreateItem(order, product));
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
                ProductName = "Product" + RandomString(),
                ProductDescription = "Product Description" + RandomString()
            };
            _generator.Context.AddToProductSet(product);
            _generator.EntityDeleteActions.Add(context => context.DeleteObject(product));
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
            var orderItem = new OrderItem
            {
                Orders = order,
                Price = 1,
                Product = product,
                Quantity = 3
            };
            _generator.EntityDeleteActions.Add(context => context.DeleteObject(orderItem));
            return orderItem;
        }

        protected string RandomString()
        {
            return _random.Next(int.MaxValue).ToString();
        }
    }
}