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
            Console.WriteLine("CustomerId | Name | Email | City | Address | PhoneNumber ");
            foreach (var row in rows)
            {
                Console.WriteLine($"{row.CustomerId} | {row.Name} | {row.Email} | {row.City} | {row.Address} | {row.PhoneNumber} ");
            }
            Console.WriteLine("-------------------");         
        }

        public static async Task ShowCustomerDetailAsync()
        {
            // New for encryption
            using var db = new StoreContext();

            var customers = await db.Customers.AsNoTracking()
                                              .OrderBy(x => x.CustomerId)
                                              .ToListAsync();

            Console.WriteLine("-------------------");
            if (!await db.Customers.AnyAsync())
            {
                Console.WriteLine("No customers found.");
                return;
            }
            Console.WriteLine("CustomerId | Name | Email (Encrypted) | City (Encrypted) | Address (Encrypted) | PhoneNumber (Encrypted) ");
            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.CustomerId} | {customer.Name} | {customer.Email} | {customer.City} | {customer.Address} | {customer.PhoneNumber} ");
            }

            Console.WriteLine("-------------------");
            Console.WriteLine("Would you like to see decrypted data? (y/n)");
            var input = Console.ReadLine()?.Trim() ?? string.Empty;
            if (input == "y")
            {
                Console.WriteLine("-------------------");
                Console.WriteLine("Please choose customer id");
                if (!int.TryParse(Console.ReadLine(), out int cId))
                {
                    Console.WriteLine("Invalid customer id.");
                    return;
                }

                var customerToView = await db.Customers.FirstAsync(c => c.CustomerId == cId);
                var customerInfo = await db.Customers.AsNoTracking().Where(c => c.CustomerId == cId).ToListAsync();

                Console.WriteLine("-------------------");
                Console.WriteLine("Please enter password to view sensetive info");
                var passwordInput = Console.ReadLine()?.Trim() ?? string.Empty;
                if (passwordInput != EncryptionHelper.Decrypt(customerToView.Password))
                {
                    Console.WriteLine("Invalid password");
                    return;
                }
                else
                {
                    Console.WriteLine("-------------------");
                    Console.WriteLine("CustomerId | Name | Email (Decrypted) | City (Decrypted) | Address (Decrypted) | PhoneNumber (Decrypted) ");
                    foreach (var customer in customerInfo)
                    {
                        Console.WriteLine($"{customer.CustomerId} | {customer.Name} | {EncryptionHelper.Decrypt(customer.Email)} | {EncryptionHelper.Decrypt(customer.City)} | {EncryptionHelper.Decrypt(customer.Address)} | {EncryptionHelper.Decrypt(customer.PhoneNumber)} ");
                    }
                }
            }
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

            Console.WriteLine("Enter customer address: ");
            var address = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(address) || address.Length > 100)
            {
                Console.WriteLine("Address is required (Max 100)");
                Console.WriteLine("----------------------------");
                return;
            }

            Console.WriteLine("Enter customer phone number: ");
            var phoneNumber = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length > 100)
            {
                Console.WriteLine("Phone Number is required (Max 100)");
                Console.WriteLine("----------------------------");
                return;
            }

            Console.WriteLine("Enter customer password: ");
            var password = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(password) || password.Length > 100)
            {
                Console.WriteLine("Password is required (Max 100)");
                Console.WriteLine("----------------------------");
                return;
            }

            using var db = new StoreContext();

            // Add new customer to DB with encrypted data
            db.Customers.Add(new Customer 
            {   
                Name = name, 
                Email = EncryptionHelper.Encrypt(email), 
                City = EncryptionHelper.Encrypt(city), 
                Address = EncryptionHelper.Encrypt(address), 
                PhoneNumber = EncryptionHelper.Encrypt(phoneNumber), 
                Password = EncryptionHelper.Encrypt(password) 
            }); 
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
