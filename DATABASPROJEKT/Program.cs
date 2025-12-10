using System.Xml.Linq;
using DATABASPROJEKT;
using DATABASPROJEKT.Enum;
using DATABASPROJEKT.Models;
using Microsoft.EntityFrameworkCore;
using DATABASPROJEKT.Helpers;

Console.WriteLine("Welcome To Youre ShoppingApp!");

await MainMenuAsync();

// Main Menu
static async Task MainMenuAsync()
{
    while (true)
    {
        Console.WriteLine("\n=============================");
        Console.WriteLine("=== Main Menu ===");
        Console.WriteLine("\n Commands: 1. Customer | 2. Orders | 3. Products | 4. Categories | 5. Exit ");
        Console.WriteLine("Enter your choice: ");
        Console.WriteLine("> ");
        var line = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(line))
            break;
        if (line.Equals("5", StringComparison.OrdinalIgnoreCase))
            break;
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var cmd = parts[0].ToLowerInvariant();
        switch (cmd)
        {
            case "1":
            case "customer":
                await CustomerAsync();
                break;
            case "2":
            case "orders":
                await OrdersAsync();
                break;
            case "3":
            case "products":
                await ProductsAsync();
                break;
            case "4":
            case "categorys":
                await CategoriesAsync();
                break;
            default:
                Console.WriteLine("Unknown command. Please try again.");
                break;
        }
        break;
    }
    return;
}

// Customer Menu
static async Task CustomerAsync()
{
    while (true)
    {
        Console.WriteLine("\n=============================");
        Console.WriteLine("\n=== Customer Management ===");
        Console.WriteLine("Commands: 1. List Customers | 2. Add Customer | 3. Edit Customer <ID> | 4. Delete Customer <ID> | 5. Back to Main Menu");
        Console.WriteLine("Enter your choice: ");
        var line = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(line))
            continue;
        if (line.Equals("5", StringComparison.OrdinalIgnoreCase))
        {
            await MainMenuAsync();
            break;
        }
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var cmd = parts[0].ToLowerInvariant();
        switch (cmd)
        {
            case "1":
            case "listcustomers":
                await CustomerHelper.ShowCustomersAsync();
                break;
            case "2":
            case "addcustomer":
                await CustomerHelper.AddCustomerAsync();
                break;
            case "3":
            case "editcustomer":
                await CustomerHelper.EditCustomerAsync(int.Parse(parts[1]));
                break;
            case "4":
            case "deletecustomer":
                if (parts.Length < 2 || !int.TryParse(parts[1], out var id))
                {
                    Console.WriteLine("Usage: Delete <id>");
                    break;
                }
                await CustomerHelper.DeleteCustomerAsync(id);
                break;
            default:
                Console.WriteLine("Unknown command. Please try again.");
                break;
        }
    }
}

// Orders Menu
static async Task OrdersAsync()
{
    while (true)
    {
        Console.WriteLine("\n=============================");
        Console.WriteLine("\n=== Order Management ===");
        Console.WriteLine("Commands: 1. List Orders | 2. Add Order | 3. Edit Order <ID> | 4. Delete Order <ID> | 5. Show Order Details <ID> | 6. Back to Main Menu");
        Console.WriteLine("Enter your choice: ");
        var line = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(line))
            continue;
        if (line.Equals("6", StringComparison.OrdinalIgnoreCase))
        {
            await MainMenuAsync();
            break;
        }
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var cmd = parts[0].ToLowerInvariant();
        switch (cmd)
        {
            case "1":
            case "listorders":
                await OrderHelper.ShowOrdersAsync();
                break;
            case "2":
            case "addorder":
                await OrderHelper.AddOrderAsync();
                break;
            case "3":
            case "editorder":
                if (parts.Length < 2 || !int.TryParse(parts[1], out var idD))
                {
                    Console.WriteLine("Usage: <id>");
                    break;
                }
                await OrderHelper.EditOrderAsync(idD);
                break;
            case "4":
            case "deleteorder":
                if (parts.Length < 2 || !int.TryParse(parts[1], out var id))
                {
                    Console.WriteLine("Usage: Delete <id>");
                    break;
                }
                await OrderHelper.DeleteOrderAsync(id);
                break;
            case "5":
            case "showorderdetailsasync":
                if (parts.Length < 2 || !int.TryParse(parts[1], out var DiD))
                {
                    Console.WriteLine("Usage: <id>");
                    break;
                }
                await OrderHelper.ShowOrderDetailsAsync(DiD);
                break;
            default:
                Console.WriteLine("Unknown command. Please try again.");
                break;
        }
    }
}

// Products Menu
static async Task ProductsAsync()
{
    while (true)
    {
        Console.WriteLine("\n=============================");
        Console.WriteLine("\n=== Product Management ===");
        Console.WriteLine("Commands: 1. List Products | 2. Add Product | 3. Edit Product <ID> | 4. Delete Product <ID> | 5. Show Product Details <ID> | 6. Back to Main Menu");
        Console.WriteLine("Enter your choice: ");
        var line = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(line))
            continue;
        if (line.Equals("6", StringComparison.OrdinalIgnoreCase))
        {
            await MainMenuAsync();
            break;
        }

        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var cmd = parts[0].ToLowerInvariant();
        switch (cmd)
        {
            case "1":
            case "listproducts":
                await ProductHelper.ShowProductsAsync();
                break;
            case "2":
            case "addproduct":
                await ProductHelper.AddProductAsync();
                break;
            case "3":
            case "editproduct":
                if (parts.Length < 2 || !int.TryParse(parts[1], out var idD))
                {
                    Console.WriteLine("Usage: <id>");
                    break;
                }
                await ProductHelper.EditProductAsync(idD);
                break;
            case "4":
            case "deleteproduct":
                if (parts.Length < 2 || !int.TryParse(parts[1], out var id))
                {
                    Console.WriteLine("Usage: Delete <id>");
                    break;
                }
                await ProductHelper.DeleteProductAsync(id);
                break;
            case "5":
                if (parts.Length < 2 || !int.TryParse(parts[1], out var productId))
                {
                    Console.WriteLine("Usage: <id>");
                    break;
                }
                await ProductHelper.ShowProductDetailsAsync(productId);
                break;
            default:
                Console.WriteLine("Unknown command. Please try again.");
                break;
        }
    } 
}

// Categories Menu
static async Task CategoriesAsync()
{
    while (true)
    {
        Console.WriteLine("\n=============================");
        Console.WriteLine("\n=== Category Management ===");
        Console.WriteLine("Commands: 1. List Categories | 2. Add Category | 3. Edit Category <ID> | 4. Delete Category <ID> | 5. Back to Main Menu");
        Console.WriteLine("Enter your choice: ");
        var line = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(line))
            continue;
        if (line.Equals("5", StringComparison.OrdinalIgnoreCase))
        {
            await MainMenuAsync();
            break;
        }
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
                if (parts.Length < 2 || !int.TryParse(parts[1], out var idD))
                {
                    Console.WriteLine("Usage: Delete <id>");
                    break;
                }
                await CategoryHelper.EditCategoryAsync(idD);
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

