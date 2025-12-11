using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DATABASPROJEKT.Enum;
using DATABASPROJEKT.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DATABASPROJEKT.Helpers
{
    public static class OrderHelper
    {
        // List all orders
        public static async Task ShowOrdersAsync()
        {
            using var db = new StoreContext();
            var orders = await db.Orders
                .AsNoTracking()
                .Include(x => x.Customer) // Eager loading of Customer
                .OrderBy(x => x.OrderId)
                .ToListAsync();

            Console.WriteLine("-------------------");
            Console.WriteLine(" OrderId | OrderDate | CustomerName | Status | TotalAmount ");
            foreach (var order in orders)
            {
                Console.WriteLine($"{order.OrderId} | {order.OrderDate.ToShortDateString()} | {order.Customer?.Name} | {order.Status} | {order.TotalAmount:C} ");
            }
            Console.WriteLine("-------------------");
        }

        // Show details of a specific order
        public static async Task ShowOrderDetailsAsync(int DiD)
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
            Console.WriteLine($"Category: {order.Categorie}");
            Console.WriteLine("Order lines:");
            if (order.OrderRows != null && order.OrderRows.Any())
            {
                foreach (var row in order.OrderRows)
                {
                    var productName = row.Product?.ProductName ?? $"ProductId:{row.ProductId}";
                    var lineTotal = row.UnitPrice * row.Quantity;
                    Console.WriteLine($" - {productName} | Qty: {row.Quantity} | UnitPrice: {row.UnitPrice:C} | LineTotal: {lineTotal:C}| Category: {row.Categorie} ");
                }
            }
            else
            {
                Console.WriteLine(" (No order rows)");
            }
            Console.WriteLine("-------------------");
        }

        // Add a new order
        public static async Task AddOrderAsync()
        {
            using var db = new StoreContext();

            // Show existing customers to choose from
            Console.WriteLine("Current Customers: ");
            await CustomerHelper.ShowCustomersAsync();

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

            //Console.WriteLine("Choose Order Status:");
            //foreach (var value in Enum.GetValues(typeof(Status)))
            //{
            //    Console.WriteLine($"{(int)value}. {value}");
            //}

            //Console.Write("> ");

            //if (!int.TryParse(Console.ReadLine(), out var input) ||
            //    !Enum.IsDefined(typeof(Status), input))
            //{
            //    Console.WriteLine("Invalid status selection.");
            //    Console.WriteLine("----------------------------");
            //    return;
            //}

            Status choice = (Status)input;

            // Add Products to order
            var orderRows = new List<OrderRow>();
            while (true)
            {
                Console.WriteLine("Current Products: ");
                await ProductHelper.ShowProductsAsync();

                Console.WriteLine("Enter Product ID to add to order (or leave blank to finish): ");
                var prodInput = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(prodInput))
                    break;

                if (!int.TryParse(prodInput, out var productId))
                {
                    Console.WriteLine("Invalid Product ID.");
                    Console.WriteLine("----------------------------");
                    continue;
                }

                var product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
                if (product == null)
                {
                    Console.WriteLine("Product not found.");
                    Console.WriteLine("----------------------------");
                    continue;
                }

                Console.WriteLine($"Enter quantity for {product.ProductName}: ");
                if (!int.TryParse(Console.ReadLine(), out var quantity) || quantity <= 0)
                {
                    Console.WriteLine("Invalid quantity.");
                    Console.WriteLine("----------------------------");
                    continue;
                }

                if (quantity > product.StockQuantity)
                {
                    Console.WriteLine("Not enough in stock.");
                    Console.WriteLine("----------------------------");
                    continue;
                }

                orderRows.Add(new OrderRow
                {
                    ProductId = product.ProductId,
                    Quantity = quantity,
                    //Status = choice,
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

        // Filter by Status
        public static async Task OrdersByStatusAsync(Status status)
        {
            using var db = new StoreContext();
         
            var orderByStatus = db.Orders
                .Include(c => c.Customer)
                .AsNoTracking()
                .Where(o => o.Status == status)
                .OrderBy(o => o.OrderDate);

            var Orders = await orderByStatus
                            .ToListAsync();

            foreach (var order in Orders)
                {
                Console.WriteLine($"{order.OrderId} - {order.CustomerId} - {order.OrderDate} | {order.Customer?.Name} | {order.Status}");
            }
        }

        // Sort by Customer ID
        public static async Task OrdersByCustomerAsync()
        {
            using var db = new StoreContext();

            var orderBy = db.Orders
                .Include(c => c.Customer)
                .AsNoTracking()
                .OrderBy(c => c.CustomerId);

            var Customer = await orderBy
                            .ToListAsync();

            foreach (var customerOrder in Customer)
            {
                Console.WriteLine($"{customerOrder.OrderId} - {customerOrder.CustomerId} - {customerOrder.OrderDate} | {customerOrder.Customer?.Name}");
            }
        }

        // Lista orders sida för sida med Skip och Take, sorterade t.ex.på OrderDate. + ADD I CASE!!!!
        public static async Task OrdersPageAsync(int page, int pageSize)
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

        // List all order summaries
        public static async Task ListOrderSummary()
        {
            using var db = new StoreContext();

            var summaries = await db.OrderSummaries.OrderByDescending(e => e.OrderDate).ToListAsync();

            Console.WriteLine("OrderId | OrderDate | TotalAmount SEK | Customer Email");
            foreach (var summary in summaries)
            {
                Console.WriteLine($"{summary.OrderId} | {summary.OrderDate} | {summary.TotalAmount} | {summary.CustomerEmail}");
            }
        }

        // Show details of a specific order summary
        public static async Task ShowOrderSummaryDetailsAsync()
        {
            Console.WriteLine("Current Order Summaries: ");
            await ListOrderSummary();
            Console.WriteLine("Enter Order ID to view summary details: ");
            if (!int.TryParse(Console.ReadLine(), out var orderId))
            {
                Console.WriteLine("Invalid Order ID.");
                Console.WriteLine("----------------------------");
                return;
            }
            using var db = new StoreContext();
            var summary = await db.OrderSummaries.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (summary == null)
            {
                Console.WriteLine("Order summary not found.");
                Console.WriteLine("----------------------------");
                return;
            }
            Console.WriteLine("-------------------");
            Console.WriteLine($"Order ID: {summary.OrderId}");
            Console.WriteLine($"Order Date: {summary.OrderDate.ToShortDateString()}");
            Console.WriteLine($"Total Amount: {summary.TotalAmount:C}");
            Console.WriteLine($"Customer Email: {summary.CustomerEmail}");
            Console.WriteLine("-------------------");
        }

        // Delete order and its associated order rows
        public static async Task DeleteOrderAsync(int orderId)
        {
            using var db = new StoreContext();
            var order = await db.Orders
                .Include(o => o.OrderRows)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                Console.WriteLine("Order not found.");
                Console.WriteLine("----------------------------");
                return;
            }
            db.OrderRows.RemoveRange(order.OrderRows);
            db.Orders.Remove(order);
            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Order deleted successfully!");
                Console.WriteLine("----------------------------");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("DB ERROR: " + ex.GetBaseException().Message);
                Console.WriteLine("----------------------------");
            }
        }

        // Update order status
        public static async Task UpdateOrderStatusAsync(int orderId, Status newStatus)
        {
            using var db = new StoreContext();
            var order = await db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                Console.WriteLine("Order not found.");
                Console.WriteLine("----------------------------");
                return;
            }
            order.Status = newStatus;
            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Order status updated successfully!");
                Console.WriteLine("----------------------------");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("DB ERROR: " + ex.GetBaseException().Message);
                Console.WriteLine("----------------------------");
            }
        }

        // Show all orders with their details
        public static async Task ShowOrdersWithDetailsAsync()
        {
            using var db = new StoreContext();
            var orders = await db.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.OrderRows)
                    .ThenInclude(or => or.Product)
                .OrderBy(o => o.OrderId)
                .ToListAsync();
            Console.WriteLine("-------------------");
            foreach (var order in orders)
            {
                Console.WriteLine($"Order ID: {order.OrderId} | Customer: {order.Customer?.Name} | Date: {order.OrderDate.ToShortDateString()} | Status: {order.Status} | Total: {order.TotalAmount:C}");
                Console.WriteLine(" Order Lines:");
                foreach (var row in order.OrderRows)
                {
                    var productName = row.Product?.ProductName ?? $"ProductId:{row.ProductId}";
                    var lineTotal = row.UnitPrice * row.Quantity;
                    Console.WriteLine($"Product Name: {productName} | Quantity: {row.Quantity} | Unit Price: {row.UnitPrice:C} | LineTotal: {lineTotal:C}");
                }
                Console.WriteLine("-------------------");
            }
        }

        public static async Task<int> GetTotalOrderCountAsync()
        {
            using var db = new StoreContext();
            return await db.Orders.CountAsync();
        }

        // Edit existing order
        public static async Task EditOrderAsync(int idD)
        {
            using var db = new StoreContext();
            // Get OrderId to edit the chosen order
            var order = await db.Orders.FirstOrDefaultAsync(x => x.OrderId == idD);
            if (order == null)
            {
                Console.WriteLine("Order not found");
                Console.WriteLine("----------------------------");
                return;
            }
            // Implement editing logic here
            order.OrderId = idD;
            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Order edited successfully!");
                Console.WriteLine("----------------------------");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine("----------------------------");
            }
        }
    }
}
