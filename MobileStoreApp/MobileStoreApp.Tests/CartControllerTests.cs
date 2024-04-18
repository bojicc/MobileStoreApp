using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MobileStoreApp.Controllers;
using MobileStoreApp.Data;
using MobileStoreApp.Data.Models;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileStoreApp.Services;

namespace MobileStoreApp.Tests
{
    public class CartControllerTests
    {
        [Fact]
        public async Task RemoveFromCart_ReturnsRedirectToActionResult_WhenOrderItemExists()
        {
            //Arrange
            int orderItemId = 1;
            var mockContext = new Mock<ApplicationDbContext>();
            var mockUserManager = MockUserManager<ApplicationUser>();
            var mockCartService = new Mock<ICartService>();
            var controller = new CartController(mockContext.Object, mockUserManager.Object, mockCartService.Object);

            mockContext.Setup(c => c.OrderItems.FindAsync(orderItemId))
                       .ReturnsAsync(new OrderItem()); // Assuming OrderItem is your entity class

            // Act
            var result = await controller.RemoveFromCart(orderItemId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(CartController.Index), redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task RemoveFromCart_ReturnsNotFound_WhenOrderItemDoesNotExist()
        {
            // Arrange
            int orderItemId = 1;
            var mockContext = new Mock<ApplicationDbContext>();
            var mockUserManager = MockUserManager<ApplicationUser>();
            var mockCartService = new Mock<ICartService>();
            var controller = new CartController(mockContext.Object, mockUserManager.Object, mockCartService.Object);

            mockContext.Setup(c => c.OrderItems.FindAsync(orderItemId))
                       .ReturnsAsync((OrderItem)null);

            // Act
            var result = await controller.RemoveFromCart(orderItemId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var userStore = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(userStore.Object, null, null, null, null, null, null, null, null);
        }
    }
}
