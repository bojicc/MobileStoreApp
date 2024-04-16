using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileStoreApp.Data.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public string UserId { get; set; }
        [Required]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        public decimal TotalPrice { get; set; }

        public bool Shipped { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
