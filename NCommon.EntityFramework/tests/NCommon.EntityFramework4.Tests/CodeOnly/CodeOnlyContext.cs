using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.RegisterSet<Customer>("Customers");
            modelBuilder.RegisterSet<Order>("Orders");
            modelBuilder.RegisterSet<OrderItem>("OrderItems");
            modelBuilder.RegisterSet<Product>("Products");
        }

        public ObjectContext Context
        {
            get { return base.ObjectContext; }
        }
    }
}