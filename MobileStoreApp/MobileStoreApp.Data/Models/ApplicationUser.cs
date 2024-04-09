using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MobileStoreApp.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string CustomTag { get; set; }
    }
}
