using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DATABASPROJEKT.Models;
using Microsoft.EntityFrameworkCore;

namespace DATABASPROJEKT
{
    public class StoreContext : DbContext
    {
        // DbSet for each modelclass
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderRow> OrderRows => Set<OrderRow>();
        public DbSet<OrderSummary> OrderSummaries => Set<OrderSummary>();

        // Here we tell EF Core that we want to use SQLite and where the file should be located
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "shop.db");
            optionsBuilder.UseSqlite("Data Source = shop.db");
        }

        // OnModelCreating is used to configure the models
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // OrderSummary
            modelBuilder.Entity<OrderSummary>(e =>
            {
                e.HasNoKey(); // Missing primary key
                e.ToView("OrderSummaryView"); // Map to SQLITE
            });

            // Customer
            modelBuilder.Entity<Customer>(e =>
            {
                // PK
                e.HasKey(c => c.CustomerId);

                // Properties
                e.Property(c => c.Name).IsRequired().HasMaxLength(100);
                e.Property(c => c.Email).IsRequired().HasMaxLength(100);
                e.Property(c => c.City).IsRequired().HasMaxLength(100);

                // One Customer can have many Orders
                e.HasMany(c => c.Orders);
            });

            // Product
            modelBuilder.Entity<Product>(e =>
            {
                // PK
                e.HasKey(p => p.ProductId);

                // Properties
                e.Property(p => p.ProductName).IsRequired().HasMaxLength(100);
                e.Property(p => p.Price).IsRequired();
                e.Property(p => p.StockQuantity).IsRequired();

                // One Product can have many OrderRows
                e.HasMany(p => p.OrderRows);
            });

            // Order
            modelBuilder.Entity<Order>(e =>
            {
                // PK
                e.HasKey(o => o.OrderId);

                // Properties
                e.Property(o => o.OrderDate).IsRequired();
                e.Property(o => o.Status).IsRequired();
                e.Property(o => o.TotalAmount).IsRequired();

                // One Order can have many OrderRows
                e.HasMany(o => o.OrderRows);

                // One Order has one Customer
                e.HasOne(o => o.Customer)
                 .WithMany(c => c.Orders)
                 .HasForeignKey(o => o.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // OrderRow
            modelBuilder.Entity<OrderRow>(e =>
            {
                // PK
                e.HasKey(or => or.OrderRowId);

                // Properties
                e.Property(x => x.UnitPrice).IsRequired();
                e.Property(x => x.Quantity).IsRequired();

                // One OrderRow has one Order
                e.HasOne(or => or.Order)
                 .WithMany(o => o.OrderRows)
                 .HasForeignKey(or => or.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);

                // One OrderRow has one Product
                e.HasOne(or => or.Product)
                 .WithMany(p => p.OrderRows)
                 .HasForeignKey(or => or.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
