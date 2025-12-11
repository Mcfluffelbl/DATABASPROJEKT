using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DATABASPROJEKT.Models;
using Microsoft.EntityFrameworkCore;

namespace DATABASPROJEKT.Helpers
{
    public static class CustomerHelper
    {
        // Show all customers
        public static async Task ShowCustomersAsync()
        {
            using var db = new StoreContext();

            // AsTracking = faster for read-only scenation. (No change tracking)
            var rows = await db.Customers.AsNoTracking().OrderBy(customers => customers.CustomerId).ToListAsync();
            Console.WriteLine("-------------------");
            Console.WriteLine("CustomerId | Name | Email | City ");
            foreach (var row in rows)
            {
                Console.WriteLine($"{row.CustomerId} | {row.Name} | {row.Email} | {row.City}");
            }
            Console.WriteLine("-------------------");
        }

        // Add new customer
        public static async Task AddCustomerAsync()
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
                // Save changes
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

        // Edit existing customer
        public static async Task EditCustomerAsync(int id)
        {
            using var db = new StoreContext();

            // Get CustomerId to edit the chosen customer
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

        // Delete existing customer
        public static async Task DeleteCustomerAsync(int id)
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

        // Sort customers by name
        public static async Task SortCustomerAsync()
        {
            using var db = new StoreContext();

            var customers = await db.Customers.AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();

            Console.WriteLine("-------------------");
            Console.WriteLine("CustomerId | Name | Email | City ");
            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.CustomerId} | {customer.Name} | {customer.Email} | {customer.City}");
            }
            Console.WriteLine("-------------------");
        }
    }
}
