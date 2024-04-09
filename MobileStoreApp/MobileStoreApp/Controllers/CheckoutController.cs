using Microsoft.AspNetCore.Mvc;

namespace MobileStoreApp.Controllers
{
    public class CheckoutController : Controller
    {
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
