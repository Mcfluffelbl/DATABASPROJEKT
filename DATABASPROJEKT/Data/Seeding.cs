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
                    new Customer { Name = "DaVinci", Email = "DaVinci@Code.com", City = "Italy", Address = EncryptionHelper.Encrypt("Trojaholm 3"), PhoneNumber = EncryptionHelper.Encrypt("0704534576"), Password = EncryptionHelper.Encrypt("1234") }, 
                    new Customer { Name = "Sten", Email = "Sten.Bergman@Telia.com", City = "Norway", Address = EncryptionHelper.Encrypt("Riddarholmen 7"), PhoneNumber = EncryptionHelper.Encrypt("0793452344"), Password = EncryptionHelper.Encrypt("4321") } 
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

            // Only if there were no products since before
            if (!await db.Products.AnyAsync())
            {
                var spik = await db.Categories.FirstAsync(c => c.CategorieName == "Spikar");
                var skruv = await db.Categories.FirstAsync(c => c.CategorieName == "Skruvar");
                db.Products.AddRange(
                new Product { ProductName = "Skruv 1.2cm", Price = 10, StockQuantity = 1, CategorieId = skruv.CategorieId },
                new Product { ProductName = "Spik 0.2cm", Price = 25, StockQuantity = 23, CategorieId = spik.CategorieId}
                );
                await db.SaveChangesAsync();
                Console.WriteLine("Seeded db!");
            }

           
        }
    }
}
