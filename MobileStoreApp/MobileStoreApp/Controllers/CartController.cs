﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileStoreApp.Data.Models;
using MobileStoreApp.Data.Repositories;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using MobileStoreApp.Data;
using Microsoft.CodeAnalysis.Scripting;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace MobileStoreApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context; 
            _userManager = userManager;
        }
        private static List<OrderItem> cartItems = new List<OrderItem>();


        public async Task<IActionResult> Index()
        {
            var orderItems = await _context.OrderItems.Include(oi => oi.Phone).ToListAsync();
            return View(orderItems);
        }


        [HttpPost]
        public async Task<IActionResult> AddToCart(int phoneId, int quantity)
        {
            var phone = await _context.Phones.FindAsync(phoneId);

            //var user = await _userManager.GetUserAsync(this.User);

            //var activeOrder = _context.Orders
            //    .Include(i => i.OrderItems)
            //    .FirstOrDefault(i => i.UserId == user.Id);

            if (phone == null)
            {
                return NotFound();
            }

            //if (quantity <= phone.Quantity)
            //{
            //    await _context.Entry(activeOrder).Reference(o => o.OrderItems).LoadAsync();

            //    // Check if the book is already in the cart
            //    var existingCartItem = activeOrder.OrderItems.FirstOrDefault(item => item.PhoneId == phoneId);
            //    if (existingCartItem != null)
            //    {
            //        existingCartItem.Quantity += quantity;
            //    }
            //    else
            //    {
            //        // If not, add it to the cart
            //        activeOrder.OrderItems.Add(new OrderItem { PhoneId = phoneId, Quantity = quantity, Phone = phone });
            //    }
            //    phone.Quantity -= quantity;
            //    await _context.SaveChangesAsync();
            //}
            //else
            //{
            //    TempData["ErrorMessage"] = $"There are only {phone.Quantity} books available.";
            //    ModelState.AddModelError("quantity", $"There are only {phone.Quantity} books available.");
            //    return RedirectToAction("Index", "Shop", new { id = phoneId });
            //}


            if (quantity > phone.Quantity)
            {
                TempData["ErrorMessage"] = "That number of phones are currenty unavailable.";
                return RedirectToAction("Index", "Shop", new { id = phoneId });
            }



            var orderItem = new OrderItem
            {
                PhoneId = phone.PhoneId,
                Quantity = quantity,
                UnitPrice = phone.Price, 
                //Phone = phone
            };
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int orderItemId)
        {
            var orderItem = await _context.OrderItems.FindAsync(orderItemId);
            if (orderItem == null)
            {
                return NotFound();
            }

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private async Task<bool> CheckAvailabilityFromDatabase(int phoneId, int quantity)
        {
            var product = await _context.Phones.FindAsync(phoneId);
            if (product != null && product.Quantity >= quantity)
            {
                return true; // Product is available
            }
            else
            {
                return false; // Product is not available
            }
        }

        public async Task<JsonResult> CheckAvailability(int phoneId, int quantity)
        {
            bool isAvailable = await CheckAvailabilityFromDatabase(phoneId, quantity);
            return Json(new { available = isAvailable });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int orderItemId, int quantity)
        {
            var cartItem = _context.OrderItems.Find(orderItemId);
            await _context.Entry(cartItem).Reference(o => o.Phone).LoadAsync();
            var phone = cartItem.Phone;

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                if (quantity > phone.Quantity)
                {
                    TempData["ErrorMessage"] = "That number of phone are unavailable.";
                    return RedirectToAction("Index", "Cart");
                }

            }

            


            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
