using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASPROJEKT.Models
{
    public class OrderSummary
    {
        public int OrderId { get; set; }

        // Properties
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
