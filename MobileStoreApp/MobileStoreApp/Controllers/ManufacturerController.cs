using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileStoreApp.Data;

namespace MobileStoreApp.Controllers
{
    public class ManufacturerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManufacturerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string manufacturerName)
        {
            var phones = await _context.Phones
                .Where(p => p.Name.ToLower().StartsWith(manufacturerName.ToLower()))
                .ToListAsync();

            return View(phones);
        }
    }
}
