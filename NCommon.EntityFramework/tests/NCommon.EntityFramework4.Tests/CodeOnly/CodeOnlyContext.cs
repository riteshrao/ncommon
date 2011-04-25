using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using NCommon.EntityFramework4.Tests.Models;

namespace NCommon.EntityFramework4.Tests.CodeOnly
{
    public class CodeOnlyContext : DbContext
    {
        public CodeOnlyContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasEntitySetName("Customers");
            modelBuilder.Entity<Order>().HasEntitySetName("Orders");
            modelBuilder.Entity<OrderItem>().HasEntitySetName("OrderItems");
            modelBuilder.Entity<Product>().HasEntitySetName("Products");
        }

        public ObjectContext Context
        {
            get { return ((IObjectContextAdapter)this).ObjectContext; }
        }
    }
}