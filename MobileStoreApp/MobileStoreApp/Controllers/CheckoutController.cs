using Microsoft.AspNetCore.Mvc;
using MobileStoreApp.Data;

namespace MobileStoreApp.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            

            return View();
        }

        public IActionResult Confirm()
        {
            return View();
        }
    }
}
