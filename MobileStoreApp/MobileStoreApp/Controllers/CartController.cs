using Microsoft.AspNetCore.Http;
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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing;
using System;
using MobileStoreApp.Services;
using Microsoft.AspNetCore.Authorization;

namespace MobileStoreApp.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartService _cartService;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ICartService cartService)
        {
            _context = context;
            _userManager = userManager;
            _cartService = cartService;
        }
        private static List<OrderItem> cartItems = new List<OrderItem>();


        public async Task<IActionResult> Index()
        {
            var orderItems = await _context.OrderItems.Include(oi => oi.Phone).Include(i => i.Order).ToListAsync();
            //int cartItemCount = orderItems.Sum(oi => oi.Quantity);
            // ViewBag.CartItemCount = cartItemCount;
            return View(orderItems);
        }


        [HttpPost]
        public async Task<IActionResult> AddToCart(int phoneId, int quantity)
        {
            var user = await _userManager.GetUserAsync(this.User);
            var activeOrder = _context.Orders
               .Include(i => i.OrderItems)
               .FirstOrDefault(i => i.UserId == user.Id);

            if (activeOrder == null)
            {
                activeOrder = new Order
                {
                    UserId = user.Id,
                    CreateDate = DateTime.Now,
                    OrderItems = new List<OrderItem>()
                };

                _context.Orders.Add(activeOrder);
                //await _context.SaveChangesAsync();
            }


            var phone = await _context.Phones.FindAsync(phoneId);

            if (phone == null)
            {
                return NotFound();
            }

            if (quantity <= phone.Quantity)
            {
                //await _context.Entry(activeOrder).Reference(o => o.OrderItems).LoadAsync();

                // Check if the book is already in the cart
                var existingCartItem = activeOrder.OrderItems.FirstOrDefault(item => item.PhoneId == phoneId);
                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += quantity;
                }
                else
                {
                    // If not, add it to the cart
                    activeOrder.OrderItems.Add(new OrderItem { PhoneId = phoneId, Quantity = quantity, Phone = phone });
                }
                //phone.Quantity -= quantity;
                await _context.SaveChangesAsync();

                //var phoneCount = activeOrder.OrderItems.Where(item => item.PhoneId == phoneId).Sum(item => item.Quantity);
                //if (phoneCount >= 4)
                //{
                //    var discount = phone.Price * 0.15m;
                //    var totalPrice = activeOrder.OrderItems.Sum(item => item.Quantity * item.Phone.Price);
                //    activeOrder.TotalPrice = totalPrice - discount;
                //    await _context.SaveChangesAsync();
                //}
                //else
                //{
                //    activeOrder.OrderItems.Sum(item => item.Quantity * item.Phone.Price);
                //    await _context.SaveChangesAsync();
                //}
                //    // Apply discount for one item
                //    var discount = phone.Price * 0.15m; // 15% discount for each phone if quantity is 4 or more
                //    var totalPrice = activeOrder.OrderItems.Sum(item => item.Quantity * item.Phone.Price);

                //    // Find the latest added item for the same phone and update its unit total price
                //    var latestCartItem = activeOrder.OrderItems.LastOrDefault(item => item.PhoneId == phoneId);
                //    if (latestCartItem != null)
                //    {
                //        latestCartItem.UnitPrice -= discount; // Apply discount to the latest added item
                //        await _context.SaveChangesAsync();
                //    }
                //}
                // Provera da li korpa sada sadrži 4 ista telefona
                //var phoneCount = activeOrder.OrderItems.Where(item => item.PhoneId == phoneId).Sum(item => item.Quantity);
                //if (phoneCount >= 4)
                //{
                //    // Primeni popust od 15% na sve proizvode iste vrste u korpi
                //    var discount = phone.Price * 0.15m; // 15% popusta za svaka 4 telefona
                //    var totalPrice = activeOrder.OrderItems.Sum(item => item.Quantity * item.Phone.Price);
                //    activeOrder.TotalPrice = totalPrice - discount;
                //    await _context.SaveChangesAsync();
                //}
            }
            else
            {
                TempData["ErrorMessage"] = $"There are only {phone.Quantity} phone available.";
                ModelState.AddModelError("quantity", $"There are only {phone.Quantity} phones available.");
                return RedirectToAction("Index", "Shop", new { id = phoneId });
            }


            //if (quantity > phone.Quantity)
            //{
            //    TempData["ErrorMessage"] = "That number of phones are currenty unavailable.";
            //    return RedirectToAction("Index", "Shop", new { id = phoneId });
            //}
            //else
            //{
            //    activeOrder.OrderItems.Add(new OrderItem { PhoneId = phoneId, Quantity = quantity, Phone = phone });
            //    await _context.SaveChangesAsync();
            //}

            //var orderItem = new OrderItem
            //{
            //    PhoneId = phone.PhoneId,
            //    Quantity = quantity,
            //    UnitPrice = phone.Price,
            //    //Phone = phone
            //};
            //_context.OrderItems.Add(orderItem);


            // Calculate the total count of items in the cart
            int cartItemCount = _context.OrderItems.Sum(item => item.Quantity);

            // Pass the cart item count to the view
            ViewData["CartItemCount"] = cartItemCount;
            //ViewBag.CartItemCount = _cartService.CartItemCount();

            return RedirectToAction("Index", "Cart");

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

        public IActionResult getCartItemCount()
        {
            return View(cartItems.Sum(item => item.Quantity));
        }

        public IActionResult Ordered(int orderItemId)
        {
            var orderItem = _context.Orders.FindAsync(orderItemId);
            if (orderItem != null)
            {
               
            }
            
            return View(orderItem);
        }
    }
}
