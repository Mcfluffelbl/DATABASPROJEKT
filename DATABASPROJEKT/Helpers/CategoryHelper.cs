using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DATABASPROJEKT.Models;
using Microsoft.EntityFrameworkCore;

namespace DATABASPROJEKT.Helpers
{
    public static class CategoryHelper
    {
        // Delete Category by Id
        public static async Task DeleteCategoryAsync(int id)
        {
            using var db = new StoreContext();
            var category = await db.Categories.FirstOrDefaultAsync(c => c.CategorieId == id);
            if (category == null)
            {
                Console.WriteLine("Category not found!");
                Console.WriteLine("----------------------------");
                return;
            }
            // Category delete
            db.Categories.Remove(category);
            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Category Deleted!");
                Console.WriteLine("----------------------------");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine("----------------------------");
            }
        }

        public static async Task ListCategoriesAsync()
        {
            using var db = new StoreContext();
            var categories = await db.Categories.AsNoTracking().ToListAsync();
            Console.WriteLine("-------------------");
            Console.WriteLine("Category Id | Category Name");
            foreach (var category in categories)
            {
                Console.WriteLine($"{category.CategorieId} | {category.CategorieName}");
            }
            Console.WriteLine("-------------------");
        }

        public static async Task EditCategoryAsync(int idD)
        {
            using var db = new StoreContext();
            // Get CategoryId to edit the chosen category
            var category = await db.Categories.FirstOrDefaultAsync(x => x.CategorieId == idD);
            if (category == null)
            {
                Console.WriteLine("Category not found");
                Console.WriteLine("----------------------------");
                return;
            }
            // Uppdate Name for specefik category
            Console.WriteLine($"Current Name: {category.CategorieName} (ID: {category.CategorieId}");
            Console.WriteLine("New Name (leave blank to keep current): ");
            var name = Console.ReadLine()?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(name))
            {
                if (name.Length > 100)
                {
                    Console.WriteLine("Name cannot exceed 100 characters.");
                    return;
                }
                category.CategorieName = name;
            }
            // Uppdate DB:N with our changes
            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Category edited and updated successfully!");
                Console.WriteLine("----------------------------");
            }
            catch (DbUpdateException exception)
            {
                Console.WriteLine("Error updating category: " + exception.GetBaseException().Message);
                Console.WriteLine("----------------------------");
            }
        }

        public static async Task AddCategoryAsync()
        {
            using var db = new StoreContext();

            // Show Category list
            Console.WriteLine("Current Categories: ");
            await ListCategoriesAsync();

            Console.WriteLine("Enter Category Name: ");
            var categoryName = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(categoryName) || categoryName.Length > 100)
            {
                Console.WriteLine("Invalid Category Name (Max 100 characters).");
                Console.WriteLine("----------------------------");
                return;
            }

            db.Categories.Add(new Categorie { CategorieName = categoryName });
            try
            {
                // Save our changes: Trigger an INSERT + all validation/constraints in the database
                await db.SaveChangesAsync();
                Console.WriteLine("Category added!");
                Console.WriteLine("----------------------------");
            }
            catch (DbUpdateException exception)
            {
                Console.WriteLine("Error adding category: " + exception.GetBaseException().Message);
                Console.WriteLine("----------------------------");
            }
            Console.WriteLine("Category added successfully!");
        }
    }
}
