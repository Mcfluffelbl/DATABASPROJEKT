using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASPROJEKT.Models
{
    public class Customer
    {
        // PK
        public int CustomerId { get; set; }

        // Properties
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;
        [Required, MaxLength(100)]
        public string Email { get; set; } = null!;
        [Required, MaxLength(100)]
        public string City { get; set; } = null!;
        [Required, MaxLength(100)]
        public string Address { get; set; } = null!;
        [Required, MaxLength(100)]
        public string PhoneNumber { get; set; } = null!;
        [Required, MaxLength(100)]
        public string Password { get; set; } = null!;

        // One Customer can have many Orders
        public List<Order> Orders { get; set; } = new();
    }
}
