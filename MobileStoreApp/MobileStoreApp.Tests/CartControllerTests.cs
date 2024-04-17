using Microsoft.AspNetCore.Mvc;
using MobileStoreApp.Controllers;
using MobileStoreApp.Data;
using MobileStoreApp.Data.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var controller = new CartController(mockContext.Object);

            mockContext.Setup(c => c.OrderItems.FindAsync(orderItemId))
                       .ReturnsAsync(new OrderItem()); // Assuming OrderItem is your entity class

            // Act
            var result = await controller.RemoveFromCart(orderItemId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(CartController.Index), redirectToActionResult.ActionName);
        }
    }
}
