using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASPROJEKT.Models
{
    public class Categorie
    {
        // PK
        public int CategorieId { get; set; }

        // Required = not allowed to be null
        // Maxlenght = generates a column with max length 100 + used in validation
        // FK
        [Required, MaxLength(100)]
        public string CategorieName { get; set; } = string.Empty;

        // One Product can have many OrderRows
        public List<OrderRow> OrderRows { get; set; } = new();

        // One Categorie can have many Products
        public List<Product> Products { get; set; } = new();
    }
}
