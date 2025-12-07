using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DATABASPROJEKT.Enum;

namespace DATABASPROJEKT.Models
{
    public class Order
    {
        // PK
        public int OrderId { get; set; }

        // FK
        public int CustomerId { get; set; }

        // Properties
        public DateTime OrderDate { get; set; }
        public Status Status { get; set; }
        public decimal TotalAmount { get; set; }

        // Acess to customer info for each order
        public Customer? Customer { get; set; }

        // One order can have multiple OrderRows
        public List<OrderRow> OrderRows { get; set; } = new();

        // Navigation property
        [ForeignKey("CategorieId")]
        public Categorie? Categorie { get; set; }
    }
}
