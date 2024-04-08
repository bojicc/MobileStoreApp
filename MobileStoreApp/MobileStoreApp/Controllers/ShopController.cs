using Microsoft.AspNetCore.Mvc;
using MobileStoreApp.Data;
using MobileStoreApp.Data.Repositories;
using System.Reflection.Metadata.Ecma335;

namespace MobileStoreApp.Controllers
{
    public class ShopController : Controller
    {
        private readonly IPhoneRepository _phoneRepository;

        public ShopController(IPhoneRepository phoneRepository)
        {
            _phoneRepository = phoneRepository;
        }

        public IActionResult Index()
        {
            var phones = _phoneRepository.GetAllPhones();
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
