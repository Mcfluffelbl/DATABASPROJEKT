using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DATABASPROJEKT.Models;
using Microsoft.EntityFrameworkCore;

namespace DATABASPROJEKT.Helpers
{
    public static class ProductHelper
    {
        public static async Task AddProductAsync()
        {
            using var db = new StoreContext();

            Console.WriteLine("Current Products: ");
            await ShowProductsAsync();

            // Let user input data for new product name
            Console.WriteLine("Enter Product Name: ");
            var productname = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(productname) || productname.Length > 100)
            {
                Console.WriteLine("Invalid Product Name.");
                Console.WriteLine("----------------------------");
                return;
            }

            Console.WriteLine("Enter Product Price: ");
            if (!decimal.TryParse(Console.ReadLine(), out var productprice))
            {
                Console.WriteLine("Invalid amount.");
                Console.WriteLine("----------------------------");
                return;
            }

            Console.WriteLine("Enter Product Quantity: ");
            if (!int.TryParse(Console.ReadLine(), out var productquantity))
            {
                Console.WriteLine("Invalid amount quantity amount.");
                Console.WriteLine("----------------------------");
                return;
            }

            Console.WriteLine("Current Categories: ");
            // Visa kategorier

            Console.WriteLine("Enter Product Category: ");
            if (!int.TryParse(Console.ReadLine(), out var categorieId))
            {
                Console.WriteLine("Invalid Category ID.");
                Console.WriteLine("----------------------------");
                return;
            }

            // Create new product
            var product = new Product
            {
                ProductName = productname,
                Price = productprice,
                StockQuantity = productquantity,
                CategorieId = categorieId
            };

            db.Products.Add(product);

            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Product added successfully!");
                Console.WriteLine("----------------------------");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("DB ERROR: " + ex.GetBaseException().Message);
                Console.WriteLine("----------------------------");
            }
        }

        public static async Task ShowProductsAsync()
        {
            using var db = new StoreContext();

            var products = await db.Products
                .AsNoTracking()
                .ToListAsync();

            Console.WriteLine("-------------------");
            Console.WriteLine("Product Id | Product Name | Product Price | Stock Quantity");
            foreach (var product in products)
            {
                Console.WriteLine($"{product.ProductId} | {product.ProductName} | {product.Price} | {product.StockQuantity}");
            }
            Console.WriteLine("-------------------");
        }
    }
}
