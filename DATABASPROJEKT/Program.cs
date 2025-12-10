using System.Xml.Linq;
using DATABASPROJEKT;
using DATABASPROJEKT.Enum;
using DATABASPROJEKT.Models;
using Microsoft.EntityFrameworkCore;
using DATABASPROJEKT.Helpers;

Console.WriteLine("DB: " + Path.Combine(AppContext.BaseDirectory, "shop.db"));
Console.WriteLine("Welcome To Youre ShoppingApp!");

// Säkerhetställ DB + migrations + seed
using (var db = new StoreContext())
{
    // Migrate Async: Skapar databasen om den inte finns
    // Kör bara om det inte finns några kategorier sen innan
    await db.Database.MigrateAsync();

    // Enkel seeding för databasen
    // Kör bara om det inte finns några Customers sen innan
    if (!await db.Customers.AnyAsync())
    {
        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(
                new Categorie { CategorieName = "Books"},
                new Categorie { CategorieName = "Movies"}
                );
            await db.SaveChangesAsync();
            Console.WriteLine("Seeded db!");
        }

        db.Customers.AddRange(
            new Customer { CustomerId = 1, Name = "DaVinci", Email = "DaVinci@Code.com", City = "Italy" },
            new Customer { CustomerId = 2, Name = "Sten", Email = "Sten.Bergman@Telia.com", City = "Norway" }
        );
        await db.SaveChangesAsync();
        Console.WriteLine("Seeded db!");
    }

    if (!await db.Products.AnyAsync())
    {
        db.Products.AddRange(
        new Product { ProductId = 1, ProductName = "Skruv", Price = 10, StockQuantity = 1, CategorieId = 1 },
        new Product { ProductId = 2, ProductName = "Spik", Price = 25, StockQuantity = 23, CategorieId = 1 }
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

while (true)
{
    Console.WriteLine("\n=============================");
    Console.WriteLine("=== ShoppingApp Main Menu ===");
    Console.WriteLine("\n Commands: 1. Customerlist | 2. View Orders | 3. Add Customer | 4. Edit Customer <ID> | 5. Delete Customer <ID> | 7. Order Details | 8. Add Order | 9. Add Product | 10. Product List | 11. ordersbystatus <status> | 12. ordersbycustomer <customerId> | 13. orderspage <page> <pageSize> | 14. Order List | 15. List Order | 0. Exit |");
    Console.WriteLine("Enter your choice: ");
    var line = Console.ReadLine()?.Trim() ?? string.Empty;

    // Jump out of loop if empty input
    if (string.IsNullOrEmpty(line))
    {
        continue;
    }

    // End loop and exit program
    if (line.Equals("0", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    // Split input into parts
    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var cmd = parts[0].ToLowerInvariant();
    // Easy switch between commands
    switch (cmd)
    {
        case "1":
        case "customerlist":
            await CustomerHelper.ShowCustomersAsync();
            break;
        case "2":
        case "vieworders":
            await OrderHelper.ShowOrdersAsync();
            break;
        case "3":
        case "addcustomer":
            await CustomerHelper.AddCustomerAsync();
            break;
        case "4":
        case "editcustomer":
            if (parts.Length < 2 || !int.TryParse(parts[1], out var id))
            {
                Console.WriteLine("Usage: Edit <id>");
                break;
            }
            await CustomerHelper.EditCustomerAsync(id);
            break;
        case "5":
        case "deletecustomer":
            // Delete a Customer
            if (parts.Length < 2 || !int.TryParse(parts[1], out var idD))
            {
                Console.WriteLine("Usage: Delete <id>");
                break;
            }
            await CustomerHelper.DeleteCustomerAsync(idD);
            break;
        case "7":
        case "orderdetails":
            await OrderHelper.ShowOrderDetailsAsync();
            break;
        case "8":
        case "addorder":
            await OrderHelper.AddOrderAsync();
            break;
        case "9":
        case "addproduct":
            await ProductHelper.AddProductAsync();
            break;
        case "10":
        case "showproduct":
            await ProductHelper.ShowProductsAsync();
            break;
        case "11":
            await OrderHelper.OrdersByStatusAsync();
            break;
        case "12":
            await OrderHelper.OrdersByCustomerAsync();
            break;
        case "13":
            if (parts.Length < 3 || !int.TryParse(parts[1], out var page) || !int.TryParse(parts[2], out var pageSize))
            {
                Console.WriteLine("Usage: orderspage <page> <pageSize>");
                break;
            }
            await OrderHelper.OrdersPageAsync(page, pageSize);
            break;
        case "14":
            await OrderHelper.ListOrderSummary();
            break;
        case "15":
            await OrderHelper.ListOrderAsync();
            break;
        default:
            Console.WriteLine("Unknown command. Please try again.");
            break;
    }
}
return;

static async Task ManageCategoriesAsync()
{
    while (true)
    {
        Console.WriteLine("\n=== Category Management ===");
        Console.WriteLine("Commands: 1. List Categories | 2. Add Category | 3. Edit Category <ID> | 4. Delete Category <ID> | 0. Back to Main Menu");
        Console.WriteLine("Enter your choice: ");
        var line = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(line))
            continue;
        if (line.Equals("0", StringComparison.OrdinalIgnoreCase))
            break;
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var cmd = parts[0].ToLowerInvariant();
        switch (cmd)
        {
            case "1":
            case "listcategories":
                await CategoryHelper.ListCategoriesAsync();
                break;
            case "2":
            case "addcategory":
                await CategoryHelper.AddCategoryAsync();
                break;
            case "3":
            case "editcategory":
                await CategoryHelper.EditCategoryAsync(int.Parse(parts[1]));
                break;
            case "4":
            case "deletecategory":
                if (parts.Length < 2 || !int.TryParse(parts[1], out var id))
                {
                    Console.WriteLine("Usage: Delete <id>");
                    break;
                }
                await CategoryHelper.DeleteCategoryAsync(id);
                break;
            default:
                Console.WriteLine("Unknown command. Please try again.");
                break;
        }
    }
}

