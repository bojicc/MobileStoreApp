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

namespace MobileStoreApp.Controllers
{
    public class CartController : Controller
    {
        private readonly IPhoneRepository _phoneRepository;
        private readonly IOrderRepository _orderRepository;

        public CartController(IPhoneRepository repository, IOrderRepository orderRepository)
        {
            _phoneRepository = repository;
            _orderRepository = orderRepository;
        }

        public IActionResult Index()
        {
            List<OrderItem> cartItems = GetCartItemsFromSession();
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            var phone = _phoneRepository.GetPhoneById(id);
            if (phone == null)
                return NotFound();

            AddPhoneToCart(phone);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            RemovePhoneFromCart(id);
            return RedirectToAction("Index");
        }

        private List<OrderItem> GetCartItemsFromSession()
{
            if (TempData.TryGetValue("CartItems", out var cartItemsJson) && cartItemsJson is string cartItemsString)
            {
                return JsonSerializer.Deserialize<List<OrderItem>>(cartItemsString) ?? new List<OrderItem>();
            }

            return new List<OrderItem>();
        }

        private void AddPhoneToCart(Phone phone)
        {
            List<OrderItem> cartItems = GetCartItemsFromSession();

            var existingItem = cartItems.FirstOrDefault(item => item.PhoneId == phone.PhoneId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cartItems.Add(new OrderItem
                {
                    PhoneId = phone.PhoneId,
                    Quantity = 1,
                    UnitPrice = phone.MyProperty
                });
            }

            TempData["CartItems"] = JsonSerializer.Serialize(cartItems);
        }

        private void RemovePhoneFromCart(int id)
        {
            List<OrderItem> cartItems = GetCartItemsFromSession();

            var itemToRemove = cartItems.FirstOrDefault(item => item.PhoneId == id);
            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
                TempData["CartItems"] = JsonSerializer.Serialize(cartItems);
            }
        }
















        //[HttpPost]
        //public IActionResult AddToCart(int phoneId, int quantity)
        //{
        //    var phone = _phoneRepository.GetPhoneById(phoneId);
        //    if (phone == null)
        //    {
        //        return NotFound(); // Phone not found
        //    }

        //    var cartItem = new CartItem
        //    {
        //        PhoneId = phoneId,
        //        Quantity = quantity,
        //        UnitPrice = phone.UnitPrice
        //    };

        //    var userId = "Get your user id here, maybe from HttpContext.User or any other source";
        //    var userCart = _orderRepository.GetActiveCartByUserId(userId);

        //    if (userCart == null)
        //    {
        //        userCart = new Cart
        //        {
        //            UserId = userId,
        //            CreateDate = DateTime.UtcNow,
        //            IsActive = true,
        //            CartItems = new List<CartItem>()
        //        };
        //        _orderRepository.AddCart(userCart);
        //    }

        //    var existingCartItem = userCart.CartItems.FirstOrDefault(ci => ci.PhoneId == phoneId);
        //    if (existingCartItem != null)
        //    {
        //        existingCartItem.Quantity += quantity;
        //    }
        //    else
        //    {
        //        userCart.CartItems.Add(cartItem);
        //    }

        //    _orderRepository.SaveChanges();

        //    return RedirectToAction("Index", "Home"); // Redirect to home page or any other page
        //}
    }


    //public IActionResult Index()
    //{
    //    List<OrderItem> cartItems = GetCartItemsFromSession();
    //    return View(cartItems);
    //}

    //[HttpPost]
    //public IActionResult AddToCart(int id)
    //{
    //    var phone = _phoneRepository.GetPhoneById(id);
    //    if (phone == null)
    //        return NotFound();

    //    AddPhoneToCart(phone);
    //    return RedirectToAction("Index");
    //}

    //[HttpPost]
    //public IActionResult UpdateQuantity(int id, int quantity)
    //{
    //    if (quantity <= 0)
    //        RemovePhoneFromCart(id);
    //    else
    //        UpdateItemQuantity(id, quantity);

    //    return RedirectToAction("Index");
    //}

    //[HttpPost]
    //public IActionResult RemoveFromCart(int id)
    //{
    //    RemovePhoneFromCart(id);
    //    return RedirectToAction("Index");
    //}

    //private List<OrderItem> GetCartItemsFromSession()
    //{
    //    var cartItems = HttpContext.Session.Get<List<OrderItem>>("CartItems");
    //    return cartItems ?? new List<OrderItem>();
    //}

    //private void AddPhoneToCart(Phone phone)
    //{
    //    List<OrderItem> cartItems = GetCartItemsFromSession();

    //    // Check if the phone is already in the cart
    //    var existingItem = cartItems.FirstOrDefault(item => item.PhoneId == phone.PhoneId);
    //    if (existingItem != null)
    //    {
    //        // If the phone is already in the cart, increment its quantity
    //        existingItem.Quantity++;
    //    }
    //    else
    //    {
    //        // If the phone is not in the cart, add it as a new item
    //        cartItems.Add(new OrderItem
    //        {
    //            PhoneId = phone.PhoneId,
    //            Quantity = 1,
    //            UnitPrice = phone.Price
    //        });
    //    }

    //    // Update the cart items in the session
    //    HttpContext.Session.Set("CartItems", cartItems);
    //}

    //private void UpdateItemQuantity(int id, int quantity)
    //{
    //    List<OrderItem> cartItems = GetCartItemsFromSession();

    //    // Find the item in the cart
    //    var itemToUpdate = cartItems.FirstOrDefault(item => item.PhoneId == id);
    //    if (itemToUpdate != null)
    //    {
    //        // Update the item's quantity
    //        itemToUpdate.Quantity = quantity;
    //    }

    //    // Update the cart items in the session
    //    HttpContext.Session.Set("CartItems", cartItems);
    //}

    //private void RemovePhoneFromCart(int id)
    //{
    //    List<OrderItem> cartItems = GetCartItemsFromSession();

    //    // Find the item in the cart
    //    var itemToRemove = cartItems.FirstOrDefault(item => item.PhoneId == id);
    //    if (itemToRemove != null)
    //    {
    //        // Remove the item from the cart
    //        cartItems.Remove(itemToRemove);
    //    }

    //    // Update the cart items in the session
    //    HttpContext.Session.Set("CartItems", cartItems);
    //}

    //[HttpPost]
    //public IActionResult AddToCart(int id)
    //{
    //    var phone = _phoneRepository.GetPhoneById(id);
    //    if (phone == null)
    //        return NotFound();

    //    AddPhoneToCart(phone);
    //    return RedirectToAction("Index");
    //}

    //[HttpPost]
    //public IActionResult RemoveFromCart(int id)
    //{
    //    RemovePhoneFromCart(id);
    //    return RedirectToAction("Index");
    //}

    //public IActionResult PlaceOrder()
    //{
    //    List<OrderItem> cartItems = GetOrderItemsFromSession();
    //    if (cartItems.Any())
    //    {
    //        Order order = new Order
    //        {
    //            UserId = "UserId", // Replace with actual user ID
    //            CreateDate = DateTime.UtcNow,
    //            OrderItems = cartItems.Select(item => new OrderItem
    //            {
    //                PhoneId = item.PhoneId,
    //                Quantity = item.Quantity,
    //                UnitPrice = item.UnitPrice
    //            }).ToList()
    //        };
    //        _orderRepository.PlaceOrder(order);
    //        ClearCart();
    //        return RedirectToAction("OrderConfirmation");
    //    }
    //    return RedirectToAction("Index");
    //}

    //public IActionResult OrderConfirmation()
    //{
    //    return View();
    //}


    //private List<OrderItem> GetOrderItemsFromSession()
    //{
    //    var cartItems = HttpContext.Session.Get<List<OrderItem>>("CartItems");
    //    return cartItems ?? new List<OrderItem>();
    //}

    //private void AddPhoneToCart(Phone phone)
    //{
    //    List<OrderItem> cartItems = GetOrderItemsFromSession();
    //    var existingItem = cartItems.FirstOrDefault(item => item.PhoneId == phone.PhoneId);
    //    if (existingItem != null)
    //    {
    //        existingItem.Quantity++;
    //    }
    //    else
    //    {
    //        cartItems.Add(new OrderItem
    //        {
    //            PhoneId = phone.PhoneId,
    //            Quantity = 1,
    //            UnitPrice = phone.MyProperty
    //        });
    //    }
    //    HttpContext.Session.Set("CartItems", cartItems);
    //}

    //private void RemovePhoneFromCart(int id)
    //{
    //    List<OrderItem> cartItems = GetOrderItemsFromSession();
    //    var itemToRemove = cartItems.FirstOrDefault(item => item.PhoneId == id);
    //    if (itemToRemove != null)
    //    {
    //        cartItems.Remove(itemToRemove);
    //        HttpContext.Session.Set("CartItems", cartItems);
    //    }
    //}

    //private void ClearCart()
    //{
    //    HttpContext.Session.Remove("CartItems");
    //}
}

