using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DATABASPROJEKT;
using DATABASPROJEKT.Enum;
using DATABASPROJEKT.Models;
using Microsoft.EntityFrameworkCore;

public static class Seeding
{
    // SeedAsync method to seed the database
    public static async Task SeedAsync()
    {
        // Show path to the database file
        Console.WriteLine("DB: " + Path.Combine(AppContext.BaseDirectory, "shop.db"));

        // Secure the DB with migrations and seed data
        using (var db = new StoreContext())
        {
            // Migrate Async: Creates database if it does not exist    
            await db.Database.MigrateAsync();

            // Easy seeding for the database
            // Only if there were no customers since before
            if (!await db.Customers.AnyAsync())
            {
                db.Customers.AddRange(
                    new Customer { CustomerId = 1, Name = "DaVinci", Email = "DaVinci@Code.com", City = "Italy" },
                    new Customer { CustomerId = 2, Name = "Sten", Email = "Sten.Bergman@Telia.com", City = "Norway" }
                );
                await db.SaveChangesAsync();
                Console.WriteLine("Seeded db!");
            }

            // Only if there were no products since before
            if (!await db.Products.AnyAsync())
            {
                db.Products.AddRange(
                new Product { ProductId = 1, ProductName = "Skruv 1.2cm", Price = 10, StockQuantity = 1, CategorieName = "Skruvar" },
                new Product { ProductId = 2, ProductName = "Spik 0.2cm", Price = 25, StockQuantity = 23, CategorieName = "Spikar" }
                );
                await db.SaveChangesAsync();
                Console.WriteLine("Seeded db!");
            }

            // Only if there were no categories since before
            if (!await db.Categories.AnyAsync())
            {
                db.Categories.AddRange(
                new Categorie { CategorieName = "Skruvar" },
                new Categorie { CategorieName = "Spikar" }
                );
                await db.SaveChangesAsync();
                Console.WriteLine("Seeded db!");
            }

            // An easy seeding for the database
            // Only if there are no orders already
            if (!await db.Orders.AnyAsync())
            {
                db.Orders.AddRange(
                new Order { OrderId = 1, OrderDate = DateTime.Today.AddDays(-3), CustomerId = 1, Status = Status.Paid, TotalAmount = 10000 },
                new Order { OrderId = 2, OrderDate = DateTime.Today.AddDays(-10), CustomerId = 2, Status = Status.Pending, TotalAmount = 200 }
                );
                await db.SaveChangesAsync();
                Console.WriteLine("Seeded db!");
            }
        }
    }
}
