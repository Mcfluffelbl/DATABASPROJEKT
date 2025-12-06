using DATABASPROJEKT;
using DATABASPROJEKT.Enum;
using DATABASPROJEKT.Models;
using Microsoft.EntityFrameworkCore;

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
        new Product { ProductId = 1, ProductName = "Skruv", Price = 10, StockQuantity = 1 },
        new Product { ProductId = 2, ProductName = "Spik", Price = 25, StockQuantity = 23 }
        );
        await db.SaveChangesAsync();
        Console.WriteLine("Seeded db!");
    }

    // Enkel  seeding för databasen
    // Kör bara om det inte finns några order sen innan
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
    Console.WriteLine("\n Commands: 1. Customerlist | 2. View Orders | 3. Add Customer | 4. Edit Customer | 5. Delete Customer | 7. Order Details | 8. Add Order | 9. Add Product | 10. Product List | 11. ordersbystatus <status> | 12. ordersbycustomer <customerId> | 13. orderspage <page> <pageSize> | 14. Order List | 15. List Order | 0. Exit |");
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
            await ShowCustomersAsync();
            break;
        case "2":
        case "vieworders":
            await ShowOrdersAsync();
            break;
        case "3":
        case "addcustomer":
            await AddCustomerAsync();
            break;
        case "4":
        case "editcustomer":
            if (parts.Length < 2 || !int.TryParse(parts[1], out var id))
            {
                Console.WriteLine("Usage: Edit <id>");
                break;
            }
            await EditCustomerAsync(id);
            break;
        case "5":
        case "deletecustomer":
            // Delete a Customer
            if (parts.Length < 2 || !int.TryParse(parts[1], out var idD))
            {
                Console.WriteLine("Usage: Delete <id>");
                break;
            }
            await DeleteCustomerAsync(idD);
            break;
        case "7":
        case "orderdetails":
            await ShowOrderDetailsAsync();
            break;
        case "8":
        case "addorder":
            await AddOrderAsync();
            break;
        case "9":
        case "addproduct":
            await AddProductAsync();
            break;
        case "10":
        case "showproduct":
            await ShowProductsAsync();
            break;
        case "11":
            await OrdersByStatusAsync();
            break;
        case "12":
            await OrdersByCustomerAsync();
            break;
        case "13":
            if (parts.Length < 3 || !int.TryParse(parts[1], out var page) || !int.TryParse(parts[2], out var pageSize))
            {
                Console.WriteLine("Usage: orderspage <page> <pageSize>");
                break;
            }
            await OrdersPageAsync(page, pageSize);
            break;
        case "14":
            await ListOrderSummary();
            break;
        case "15":
            await ListOrderAsync();
            break;
        default:
            Console.WriteLine("Unknown command. Please try again.");
            break;
    }
}
return;

static async Task ListOrderAsync()
{
    using var db = new StoreContext();

    var orders = await db.Orders.AsNoTracking()
                 .Include(o => o.Customer)
                 .Include(o => o.OrderRows)
                 .OrderByDescending(o => o.OrderId)
                 .ToListAsync();
    foreach (var order in orders)
    {
        Console.WriteLine($"{order.OrderId} | {order.Customer?.Email}");
    }
}

static async Task ListOrderSummary()
{
    using var db = new StoreContext();

    var summaries = await db.OrderSummaries.OrderByDescending(e => e.OrderDate).ToListAsync();

    Console.WriteLine("OrderId | OrderDate | TotalAmount SEK | Customer Email");
    foreach (var summary in summaries)
    {
        Console.WriteLine($"{summary.OrderId} | {summary.OrderDate} | {summary.TotalAmount} | {summary.CustomerEmail}");
    }
}

static async Task ShowCustomersAsync()
{
    using var db = new StoreContext();

    // AsNoTracking = snabbare för read-only scenation. (Ingen change tracking)
    var rows = await db.Customers.AsNoTracking().OrderBy(customers => customers.CustomerId).ToListAsync();
    Console.WriteLine("-------------------");
    Console.WriteLine("CustomerId | Name | Email | City ");
    foreach (var row in rows)
    {
        Console.WriteLine($"{row.CustomerId} | {row.Name} | {row.Email} | {row.City}");
    }
    Console.WriteLine("-------------------");
}

static async Task ShowOrdersAsync()
{
    using var db = new StoreContext();
    var orders = await db.Orders
        .AsNoTracking()
        .Include(x => x.Customer) // Eager loading av Customer
        .OrderBy(x => x.OrderId)
        .ToListAsync();

    Console.WriteLine("-------------------");
    Console.WriteLine("OrderId | OrderDate | CustomerName | Status | TotalAmount");
    foreach (var order in orders)
    {
        Console.WriteLine($"{order.OrderId} | {order.OrderDate.ToShortDateString()} | {order.Customer?.Name} | {order.Status} | {order.TotalAmount:C}");
    }
    Console.WriteLine("-------------------");
}

static async Task AddCustomerAsync()
{
    Console.WriteLine("Enter customer name: ");
    var name = Console.ReadLine()?.Trim() ?? string.Empty;
    if (string.IsNullOrEmpty(name) || name.Length > 100)
    {
        Console.WriteLine("Name is required (Max 100)");
        Console.WriteLine("----------------------------");
        return;
    }

    Console.WriteLine("Enter customer email: ");
    var email = Console.ReadLine()?.Trim() ?? string.Empty;
    if (string.IsNullOrEmpty(email) || email.Length > 100)
    {
        Console.WriteLine("Email is required (Max 100)");
        Console.WriteLine("----------------------------");
        return;
    }

    Console.WriteLine("Enter customer city: ");
    var city = Console.ReadLine()?.Trim() ?? string.Empty;
    if (string.IsNullOrEmpty(city) || city.Length > 100)
    {
        Console.WriteLine("City is required (Max 100)");
        Console.WriteLine("----------------------------");
        return;
    }

    using var db = new StoreContext();
    db.Customers.Add(new Customer { Name = name, Email = email, City = city });
    try
    {
        // Spara våra ändringar: Trigga en INSERT + all valedring/constraints i databasen
        await db.SaveChangesAsync();
        Console.WriteLine("Customer added!");
        Console.WriteLine("----------------------------");
    }
    catch (DbUpdateException exception)
    {
        Console.WriteLine("Error adding customer: " + exception.GetBaseException().Message);
        Console.WriteLine("----------------------------");
    }
    Console.WriteLine("Customer added successfully!");
}

static async Task EditCustomerAsync(int id)
{
    using var db = new StoreContext();

    // Get CustomerId to edit
    var customer = await db.Customers.FirstOrDefaultAsync(x => x.CustomerId == id);
    if (customer == null)
    {
        Console.WriteLine("Customer not found");
        Console.WriteLine("----------------------------");
        return;
    }

    // Uppdate Name for specefik customer
    Console.WriteLine($"Current Name: {customer.Name} (ID: {customer.CustomerId}");
    Console.WriteLine("New Name (leave blank to keep current): ");
    var name = Console.ReadLine()?.Trim() ?? string.Empty;
    if (!string.IsNullOrEmpty(name))
    {
        if (name.Length > 100)
        {
            Console.WriteLine("Name cannot exceed 100 characters.");
            return;
        }
        customer.Name = name;
    }

    // Uppdate Email for specefik customer
    Console.WriteLine($"Current email: {customer.Email}");
    Console.WriteLine("Enter new email (leave blank to keep current): ");
    var email = Console.ReadLine()?.Trim() ?? string.Empty;
    if (!string.IsNullOrEmpty(email))
    {
        if (email.Length > 100)
        {
            Console.WriteLine("Email cannot exceed 100 characters.");
            return;
        }
        customer.Email = email;
    }

    // Uppdate City for specefik customer
    Console.WriteLine($"Current City: {customer.City}");
    Console.WriteLine("Enter new city (leave blank to keep current): ");
    var city = Console.ReadLine()?.Trim() ?? string.Empty;
    if (!string.IsNullOrEmpty(city))
    {
        if (city.Length > 100)
        {
            Console.WriteLine("City cannot exceed 100 characters.");
            return;
        }
        customer.City = city;
    }

    // Uppdate DB:N with our changes
    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Customer edited and updated successfully!");
        Console.WriteLine("----------------------------");
    }
    catch (DbUpdateException exception)
    {
        Console.WriteLine("Error updating customer: " + exception.GetBaseException().Message);
        Console.WriteLine("----------------------------");
    }
}

static async Task DeleteCustomerAsync(int id)
{
    using var db = new StoreContext();
    var customer = await db.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
    if (customer == null)
    {
        Console.WriteLine("Customer not found!");
        Console.WriteLine("----------------------------");
        return;
    }

    // Customer delete
    db.Customers.Remove(customer);
    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Customer Deleted!");
        Console.WriteLine("----------------------------");
    }
    catch (Exception exception)
    {
        Console.WriteLine(exception.Message);
        Console.WriteLine("----------------------------");
    }
}

static async Task ShowOrderDetailsAsync()
{
    Console.WriteLine("Current Orders: ");
    await ShowOrdersAsync();

    Console.WriteLine("Enter Order ID to view details: ");
    if (!int.TryParse(Console.ReadLine(), out var orderId))
    {
        Console.WriteLine("Invalid Order ID.");
        Console.WriteLine("----------------------------");
        return;
    }

    using var db = new StoreContext();

    // Load a single order including customer and order row -> product details
    var order = await db.Orders
        .AsNoTracking()
        .Include(o => o.Customer)
        .Include(o => o.OrderRows)
            .ThenInclude(or => or.Product)
        .FirstOrDefaultAsync(o => o.OrderId == orderId);

    if (order == null)
    {
        Console.WriteLine("Order not found.");
        Console.WriteLine("----------------------------");
        return;
    }

    // Lägg till så att det finns : Sortering och filtrering: o ordersbystatus<status> → filtrera orders på t.ex. "Pending", "Paid", "Shipped" o ordersbycustomer<customerId> → visa alla orders för en viss kund


    // Paginering:
    // o orderspage<page> < pageSize > → lista orders sida för sida med Skip och
    // Take, sorterade t.ex.på OrderDate.


    Console.WriteLine("-------------------");
    Console.WriteLine($"Order ID: {order.OrderId}");
    Console.WriteLine($"Order Date: {order.OrderDate.ToShortDateString()}");
    Console.WriteLine($"Customer Name: {order.Customer?.Name}");
    Console.WriteLine($"Status: {order.Status}");
    Console.WriteLine($"Total Amount: {order.TotalAmount:C}");
    Console.WriteLine("Order lines:");
    if (order.OrderRows != null && order.OrderRows.Any())
    {
        foreach (var row in order.OrderRows)
        {
            var productName = row.Product?.ProductName ?? $"ProductId:{row.ProductId}";
            var lineTotal = row.UnitPrice * row.Quantity;
            Console.WriteLine($" - {productName} | Qty: {row.Quantity} | UnitPrice: {row.UnitPrice:C} | LineTotal: {lineTotal:C}");
        }
    }
    else
    {
        Console.WriteLine(" (No order rows)");
    }
    Console.WriteLine("-------------------");
}

static async Task AddOrderAsync()
{
    using var db = new StoreContext();

    // Show existing customers to choose from
    await ShowCustomersAsync();

    // Let user input data for new order, asks for CustomerId
    Console.WriteLine("Enter Customer ID for the order: ");
    if (!int.TryParse(Console.ReadLine(), out var customerId))
    {
        Console.WriteLine("Invalid Customer ID.");
        Console.WriteLine("----------------------------");
        return;
    }

    // Let user input date data for new order
    Console.WriteLine("Enter Order Date (yyyy-MM-dd): ");
    if (!DateTime.TryParse(Console.ReadLine(), out var orderDate))
    {
        Console.WriteLine("Invalid date format.");
        Console.WriteLine("----------------------------");
        return;
    }

    Console.WriteLine("Choose Order Status:");
    foreach (var value in Enum.GetValues(typeof(Status)))
    {
        Console.WriteLine($"{(int)value}. {value}");
    }

    Console.Write("> ");

    if (!int.TryParse(Console.ReadLine(), out var input) ||
        !Enum.IsDefined(typeof(Status), input))
    {
        Console.WriteLine("Invalid status selection.");
        return;
    }

    Status choice = (Status)input;

    // Add Products to order
    var orderRows = new List<OrderRow>();
    while (true)
    {
        await ShowProductsAsync();
        Console.WriteLine("Enter Product ID to add to order (or leave blank to finish): ");
        var prodInput = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(prodInput))
            break;

        if (!int.TryParse(prodInput, out var productId))
        {
            Console.WriteLine("Invalid Product ID.");
            continue;
        }

        var product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
        if (product == null)
        {
            Console.WriteLine("Product not found.");
            continue;
        }

        Console.WriteLine($"Enter quantity for {product.ProductName}: ");
        if (!int.TryParse(Console.ReadLine(), out var quantity) || quantity <= 0)
        {
            Console.WriteLine("Invalid quantity.");
            continue;
        }

        if (quantity > product.StockQuantity)
        {
            Console.WriteLine("Not enough in stock.");
            continue;
        }

        orderRows.Add(new OrderRow
        {
            ProductId = product.ProductId,
            Quantity = quantity,
            Status = choice,
            UnitPrice = product.Price
        });

        Console.WriteLine("Add another product? (y/n): ");
        var another = Console.ReadLine()?.Trim().ToLowerInvariant();
        if (another != "y")
            break;
    }
    if (orderRows.Count == 0)
    {
        Console.WriteLine("No products added to order.");
        Console.WriteLine("----------------------------");
        return;
    }

    // Calculate and save TotalAmount correctly
    var totalAmount = orderRows.Sum(row => row.UnitPrice * row.Quantity);

    // Create new order object and attach order rows
    var order = new Order
    {
        CustomerId = customerId,
        OrderDate = orderDate,
        Status = choice,
        TotalAmount = totalAmount,
        OrderRows = orderRows
    };

    db.Orders.Add(order);

    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Order added successfully!");
        Console.WriteLine("----------------------------");
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine("DB ERROR: " + ex.GetBaseException().Message);
        Console.WriteLine("----------------------------");
    }
}

static async Task AddProductAsync()
{
    using var db = new StoreContext();

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

    // Create new product
    var product = new Product
    {
        ProductName = productname,
        Price = productprice,
        StockQuantity = productquantity
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

static async Task ShowProductsAsync()
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

static async Task OrdersByStatusAsync()
{
    using var db = new StoreContext();

    // Filtrera orders på t.ex. "Pending", "Paid", "Shipped"

}

static async Task OrdersByCustomerAsync()
{
    using var db = new StoreContext();

    // Visa alla orders för en viss kund
    Console.WriteLine("");
}

// Lista orders sida för sida med Skip och Take, sorterade t.ex.på OrderDate. + ADD I CASE!!!!
static async Task OrdersPageAsync(int page, int pageSize)
{
    using var db = new StoreContext();

    // LINQ - Sort + Ordering
    var query = db.Orders
                    .Include(b => b.Customer)
                    .AsNoTracking()
                    .OrderBy(b => b.OrderDate);

    var totalCount = await query.CountAsync();
    var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

    var Orders = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

    Console.WriteLine($"Page {page}/{totalPages}, pagesize={pageSize}");

    foreach (var order in Orders)
    {
        Console.WriteLine($"{order.OrderId} - {order.CustomerId} - {order.OrderDate} | {order.Customer?.Name}");
    }
}