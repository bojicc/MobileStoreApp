using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileStoreApp.Data.Models
{
    public class Phone
    {
        [Key]
        public int PhoneId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name="Operation System")]
        public string OperationSystem { get; set; }

        public string Picture { get; set; }         

        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
