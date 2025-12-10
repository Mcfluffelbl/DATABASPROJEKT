using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DATABASPROJEKT.Enum;

namespace DATABASPROJEKT.Models
{
    public class OrderRow
    {
        // PK
        public int OrderRowId { get; set; }

        // FK
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public Status? Status { get; set; }
        public int? CategorieId { get; set; }

        // Properties
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }

        // Access to order info for each order line
        public Order? Order { get; set; }

        // Access to product info for each order line
        public Product? Product { get; set; }

        //Access to category info for each order line
        public Categorie? Categorie { get; set; }
    }
}
