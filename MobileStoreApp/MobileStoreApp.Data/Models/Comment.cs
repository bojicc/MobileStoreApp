using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileStoreApp.Data.Models
{
    public class Comment
    {
        [Key]
        public int CommnetId { get; set; }

        public string UserId { get; set; }
        public int PhoneId { get; set; }
        public string Content {  get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public ApplicationUser User { get; set; }
        public Phone Phone { get; set; }
    }
}
