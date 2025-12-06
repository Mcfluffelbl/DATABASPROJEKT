using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASPROJEKT.Models
{
    public class Product
    {
        // PK
        public int ProductId { get; set; }

        // Characteristics
        [Required, MaxLength(100)]
        public string ProductName { get; set; } = null!;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int StockQuantity { get; set; }

        // One order can have many rows
        public List<OrderRow> OrderRows { get; set; } = new();
    }
}
