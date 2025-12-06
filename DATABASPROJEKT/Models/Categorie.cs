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
        // Properties
        [Required, MaxLength(100)]
        public string CategorieName { get; set; } = null!;
        // One Categorie can have many Products
        public List<Product> Products { get; set; } = new();
    }
}
