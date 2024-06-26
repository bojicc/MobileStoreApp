﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        public IActionResult Index(string filter, string sort)
        {
            var phones = _phoneRepository.GetAllPhones();

            if (!string.IsNullOrEmpty(filter))
            {
                phones = phones.Where(p => p.OperationSystem.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 || p.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            switch (sort)
            {
                case "highestPrice":
                    phones = phones.OrderByDescending(p => p.Price).ToList();
                    break;
                case "lowestPrice":
                    phones = phones.OrderBy(p => p.Price).ToList();
                    break;
                default:
                    break;
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

        [Authorize]
        public async Task<IActionResult> AddComment(int phoneId, string content, int rating)
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
                Rating = rating,
                CreatedDate = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Shop", new {id = phoneId});
        }

        public IActionResult Compare(int id)
        {
            var phone = _context.Phones.FirstOrDefault(p => p.PhoneId == id);
            if (phone == null)
                return NotFound();

            return View(phone);
        }

        public IActionResult AllPhones()
        {
            var phones = _phoneRepository.GetAllPhones(); 
            return PartialView("_AllPhonesPartial", phones); 
        }

        public IActionResult Comparison(int id)
        {
            var phone = _phoneRepository.GetPhoneById(id); 
            if (phone == null)
            {
                return NotFound();
            }
            return View(phone);
        }
    }
}