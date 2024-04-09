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

namespace MobileStoreApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context; 
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
            if (phone == null)
            {
                return NotFound();
            }

            //if (quantity > phone.Quantity)
            //{
            //    return View("../Shop/Details");
            //}


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


    }
}
