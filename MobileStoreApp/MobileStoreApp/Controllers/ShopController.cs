using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileStoreApp.Data;
using MobileStoreApp.Data.Repositories;
using System.Reflection.Metadata.Ecma335;

namespace MobileStoreApp.Controllers
{
    [Authorize]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPhoneRepository _phoneRepository;

        public ShopController(ApplicationDbContext context, IPhoneRepository phoneRepository)
        {
            _context = context; 
            _phoneRepository = phoneRepository;
        }

        public IActionResult Index(string filter)
        {
            
            var phones = _phoneRepository.GetAllPhones();

            if (!string.IsNullOrEmpty(filter))
            {
                phones = phones.Where(p => p.OperationSystem.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 || p.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                
            }
            return View(phones);
        }

        public IActionResult Details(int id) 
        {
            var phone = _phoneRepository.GetPhoneById(id);
            if(phone == null)
                return NotFound();

            return View(phone);
        }
    }
}
