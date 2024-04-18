using Microsoft.AspNetCore.Mvc;
using MobileStoreApp.Controllers;
using MobileStoreApp.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileStoreApp.Tests
{
    public class CheckoutControllerTests
    {
        [Fact]
        public void Test_Index_ReturnsViewName()
        {
            var mockContext = new Mock<ApplicationDbContext>();
            var controller = new CheckoutController(mockContext.Object);

            var result = controller.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);
        }


        [Fact]
        public void Test_Confirm_ReturnsViewName()
        {
            var mockContext = new Mock<ApplicationDbContext>();
            var controller = new CheckoutController(mockContext.Object);
            
            var result = controller.Confirm() as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("Confirm", result.ViewName);
        }
    }
}
