using System;
using System.Linq;
using NCommon.LinqToSql.Tests.HRDomain;
using NCommon.LinqToSql.Tests.OrdersDomain;

namespace NCommon.LinqToSql.Tests
{
    public class LinqToSqlTestDataActions
    {
        readonly LinqToSqlTestData _generator;
        readonly Random _random = new Random();

        public LinqToSqlTestDataActions(LinqToSqlTestData generator)
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
            _generator.Context<OrdersDataDataContext>().Customers.InsertOnSubmit(customer);
            _generator.EntityDeleteActions.Add(context => ((OrdersDataDataContext)context).Customers.DeleteOnSubmit(customer));
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
             _generator.Context<OrdersDataDataContext>().Customers.InsertOnSubmit(customer);
             _generator.EntityDeleteActions.Add(context => ((OrdersDataDataContext)context).Customers.DeleteOnSubmit(customer));
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
            _generator.Context<OrdersDataDataContext>().Orders.InsertOnSubmit(order);
            _generator.EntityDeleteActions.Add(context => ((OrdersDataDataContext) context).Orders.DeleteOnSubmit(order));
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
                Name = "Product" + RandomString(),
                Description = "Product Description" + RandomString()
            };
            _generator.Context<OrdersDataDataContext>().Products.InsertOnSubmit(product);
            _generator.EntityDeleteActions.Add(context => ((OrdersDataDataContext)context).Products.DeleteOnSubmit(product));
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
                Order = order,
                Price = 1,
                Product = product,
                Quantity = 3,
                Store = "Internet"
            };
            _generator.EntityDeleteActions.Add(context => ((OrdersDataDataContext)context).OrderItems.DeleteOnSubmit(orderItem));
            return orderItem;
        }

        public Customer GetCustomerById(int customerId)
        {
            var customer = _generator.Context<OrdersDataDataContext>().Customers
                .Where(x => x.CustomerID == customerId)
                .FirstOrDefault();

            if (customer != null)
                _generator.EntityDeleteActions.Add((x) => 
                    ((OrdersDataDataContext)x).Customers.DeleteOnSubmit(customer));
            return customer;
        }

        protected string RandomString()
        {
            return _random.Next(int.MaxValue).ToString();
        }

        public SalesPerson CreateSalesPerson()
        {
            var salesPerson = new SalesPerson
            {
                FirstName = "Jane" + RandomString(),
                LastName = "Doe" + RandomString(),
                SalesQuota = 100
            };
            _generator.Context<HRDataDataContext>().SalesPersons.InsertOnSubmit(salesPerson);
            return salesPerson;
        }

        public Order GetOrderById(int orderId)
        {
            var order = _generator.Context<OrdersDataDataContext>().Orders
                .Where(x => x.OrderID == orderId)
                .FirstOrDefault();

            if (order != null)
                _generator.EntityDeleteActions.Add(x => ((OrdersDataDataContext)x).Orders.DeleteOnSubmit(order));
            return order;
        }

        public SalesPerson GetSalesPersonById(int id)
        {
            var salesPerson = _generator.Context<HRDataDataContext>().SalesPersons
                .Where(x => x.Id == id)
                .FirstOrDefault();
            if (salesPerson != null)
                _generator.EntityDeleteActions.Add(x => ((HRDataDataContext)x).SalesPersons.DeleteOnSubmit(salesPerson));
            return salesPerson;
        }
    }
}