using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        // Foreign Key to Categorie
        [ForeignKey(nameof(CategorieId))]
        public int CategorieId { get; set; }
        // Navigation property
        [ForeignKey("CategorieId")]
        public Categorie? Categorie { get; set; }
    }
}
