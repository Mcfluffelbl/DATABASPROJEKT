using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASPROJEKT.Models
{
    // An easy model class (entity) that EF Core maps to a table "Category"
    public class Categorie
    {
        // PK
        public int CategorieId { get; set; }
        // Properties
        // Required = not allowed to be null
        // Maxlenght = generates a column with max length 100 + used in validation
        [Required, MaxLength(100)]
        public string CategorieName { get; set; } = string.Empty;

        // Add navigation for OrderRows so WithMany(c => c.OrderRows) compiles
        public List<OrderRow> OrderRows { get; set; } = new();

        // If you also want products
        public List<Product> Products { get; set; } = new();
    }
}
