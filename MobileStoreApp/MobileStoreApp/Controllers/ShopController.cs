using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileStoreApp.Data;
using MobileStoreApp.Data.Models;
using MobileStoreApp.Data.Repositories;
using System.Reflection.Metadata.Ecma335;

namespace MobileStoreApp.Controllers
{
   
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPhoneRepository _phoneRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShopController(ApplicationDbContext context, IPhoneRepository phoneRepository, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _phoneRepository = phoneRepository;
            _userManager = userManager;
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
            var phone = _context.Phones
                .Include(c => c.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefault(p => p.PhoneId == id);
          
            if (phone == null)
                return NotFound();

            return View(phone);
        }


        public IActionResult Compare(int id)
        {
            var phone = _phoneRepository.GetPhoneById(id);
            if (phone == null)
                return NotFound();

            return RedirectToAction("Compare", "Shop", new { id = id });
        }

        [Authorize]
        public async Task<IActionResult> AddComment(int phoneId, string content)
        {
            var user = await _userManager.GetUserAsync(this.User);
            //if(user == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            var comment = new Comment
            {
                UserId = user.Id,
                PhoneId = phoneId,
                Content = content,
                CreatedDate = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Shop", new {id = phoneId});
        }
    }
}