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
            int cartItemCount = orderItems.Sum(oi => oi.Quantity);
            ViewBag.CartItemCount = cartItemCount;
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
            }
            else
            {
                TempData["ErrorMessage"] = $"There are only {phone.Quantity} phone available.";
                ModelState.AddModelError("quantity", $"There are only {phone.Quantity} phones available.");
                return RedirectToAction("Index", "Shop", new { id = phoneId });
            }


            int cartItemCount = _context.OrderItems.Sum(item => item.Quantity);

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
                return true;
            }
            else
            {
                return false; 
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


        public async Task<IActionResult> MarkAsShipped(int orderItemId)
        {
            var user = await _userManager.GetUserAsync(this.User);

            // Pronađi aktivnu porudžbinu za trenutnog korisnika
            var activeOrder = _context.Orders.FirstOrDefault(i => i.UserId == user.Id && i.Shipped == false);

            // Kreiraj novu porudžbinu ako ne postoji aktivna porudžbina
            if (activeOrder == null)
            {
                activeOrder = new Order
                {
                    UserId = user.Id,
                    CreateDate = DateTime.Now,
                    OrderItems = new List<OrderItem>()
                };

                _context.Orders.Add(activeOrder);

            }
            else
            {
                // Obriši sve stavke iz tabele OrderItems vezane za aktivnu porudžbinu
                var orderItemsToRemove = _context.OrderItems.Where(item => item.OrderId == activeOrder.OrderId);
                _context.OrderItems.RemoveRange(orderItemsToRemove);
            }

            // Postavi status "Shipped" na true za aktivnu porudžbinu
            activeOrder.Shipped = true;

            // Izračunaj ukupnu cenu porudžbine
            decimal totalPrice = _context.OrderItems.Sum(item => item.Phone.Price * item.Quantity);
            activeOrder.TotalPrice = totalPrice;

            foreach (var orderItem in _context.OrderItems)
            {
                var phone = _context.Phones.FirstOrDefault(p => p.PhoneId == orderItem.PhoneId);
                if (phone != null)
                {
                    phone.Quantity -= orderItem.Quantity;
                }
            }


            // Sačuvaj sve promene u bazi podataka
            await _context.SaveChangesAsync();

            // Redirektuj na odgovarajuću akciju
            return RedirectToAction("Confirm", "Checkout", new { id = orderItemId });
        }





        // MOJAAAAAAAAAAAAAAAAAA********************************************************************************
        //public async Task<IActionResult> MarkAsShipped(int orderItemId)
        //{
        //    var user = await _userManager.GetUserAsync(this.User);
        //    var activeOrder = _context.Orders.FirstOrDefault(i => i.UserId == user.Id && i.Shipped == false);

        //    if (activeOrder == null)
        //    {
        //        activeOrder = new Order
        //        {
        //            UserId = user.Id,
        //            CreateDate = DateTime.Now,
        //            OrderItems = new List<OrderItem>()
        //        };
        //        _context.Orders.Add(activeOrder);
        //    }
        //    if (activeOrder != null)
        //    {

        //        activeOrder.Shipped = true;

        //        var orderItemsToRemove = _context.OrderItems.Where(item => item.OrderId == activeOrder.OrderId);
        //        _context.OrderItems.RemoveRange(orderItemsToRemove);

        //        decimal totalPrice = _context.OrderItems.Sum(item => item.Phone.Price * item.Quantity);
        //        activeOrder.TotalPrice = totalPrice;
        //        await _context.SaveChangesAsync();
        //    }
        //    return RedirectToAction("Confirm", "Checkout", new { id = orderItemId });
        //}











































        //public async Task<IActionResult> MarkAsShipped(int orderItemId)
        //{
        //    var user = await _userManager.GetUserAsync(this.User);
        //    var activeOrder = _context.Orders.FirstOrDefault(i => i.UserId == user.Id && i.Shipped == false);

        //    if (activeOrder != null)
        //    {

        //        activeOrder.Shipped = true;

        //        var orderItemsToRemove = _context.OrderItems.Where(item => item.Order.OrderId == activeOrder.OrderId);

        //        decimal totalPrice = _context.OrderItems.Sum(item => item.Phone.Price * item.Quantity);
        //        activeOrder.TotalPrice = totalPrice;

        //        _context.OrderItems.RemoveRange(orderItemsToRemove);


        //        await _context.SaveChangesAsync();
        //    }

        //    return RedirectToAction("Confirm", "Checkout", new { id = orderItemId });
        //}
    }
}
