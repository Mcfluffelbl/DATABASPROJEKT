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
        // Add new product
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

        // Show all products
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

        // Edit existing product
        public static async Task EditProductAsync(int idD)
        {
            using var db = new StoreContext();

            // Get ProductId to edit the chosen product
            var product = await db.Products.FirstOrDefaultAsync(x => x.ProductId == idD);
            if (product == null)
            {
                Console.WriteLine("Product not found");
                Console.WriteLine("----------------------------");
                return;
            }

            // Uppdate Name for specefik product
            Console.WriteLine($"Current Name: {product.ProductName} (ID: {product.ProductId}");
            Console.WriteLine("New Name (leave blank to keep current): ");
            var name = Console.ReadLine()?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(name))
            {
                if (name.Length > 100)
                {
                    Console.WriteLine("Name cannot exceed 100 characters.");
                    Console.WriteLine("----------------------------");
                    return;
                }
                product.ProductName = name;
            }
            // Uppdate DB:N with our changes
            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Product edited and updated successfully!");
                Console.WriteLine("----------------------------");
            }
            catch (DbUpdateException exception)
            {
                Console.WriteLine("Error updating product: " + exception.GetBaseException().Message);
                Console.WriteLine("----------------------------");
            }
        }

        // Show product details
        public static async Task ShowProductDetailsAsync(int productId)
        {
            using var db = new StoreContext();
            var product = await db.Products
                .AsNoTracking()
                .Include(c => c.Categorie)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                Console.WriteLine("Product not found.");
                return;
            }
            Console.WriteLine("Product Details:");
            Console.WriteLine($"ID: {product.ProductId}");
            Console.WriteLine($"Name: {product.ProductName}");
            Console.WriteLine($"Price: {product.Price}");
            Console.WriteLine($"Stock Quantity: {product.StockQuantity}");
            Console.WriteLine($"Category: {product.Categorie?.CategorieName}");
            Console.WriteLine("----------------------------");
        }

        // Delete product
        public static async Task DeleteProductAsync(int productId)
        {
            using var db = new StoreContext();
            var product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                Console.WriteLine("Product not found.");
                Console.WriteLine("----------------------------");
                return;
            }
            db.Products.Remove(product);
            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Product deleted successfully!");
                Console.WriteLine("----------------------------");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error deleting product: " + ex.GetBaseException().Message);
                Console.WriteLine("----------------------------");
            }
        }
    }
}
