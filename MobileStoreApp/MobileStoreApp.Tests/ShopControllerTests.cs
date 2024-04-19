using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileStoreApp.Controllers;
using MobileStoreApp.Data;
using MobileStoreApp.Data.Models;
using MobileStoreApp.Data.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace MobileStoreApp.Tests
{
    public class ShopControllerTests
    {
        //Index
        [Fact]
        public async Task Index_ReturnsViewResult_WithAllPhones_WhenNoFilter()
        {
            //Arrange
            var phones = new List<Phone>
            {
                new Phone { PhoneId = 1, Name = "Name 1", OperationSystem = "Android"},
                new Phone { PhoneId = 2, Name = "Name 2", OperationSystem = "IOS"},
                new Phone { PhoneId = 3, Name = "Name 3", OperationSystem = "Android"}
            };

            var mockPhoneRepository = new Mock<IPhoneRepository>();
            mockPhoneRepository.Setup(p => p.GetAllPhones()).Returns(phones);

            var controller = new ShopController(null, mockPhoneRepository.Object, null);

            //Act
            var result = controller.Index(null);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Phone>>(viewResult.ViewData.Model);
            Assert.Equal(3, model.Count());
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithFilteredPhones()
        {
            //Arrange
            var phones = new List<Phone>
            {
                new Phone { PhoneId = 1, Name = "Name 1", OperationSystem = "Android"},
                new Phone { PhoneId = 2, Name = "Name 2", OperationSystem = "IOS"},
                new Phone { PhoneId = 3, Name = "Name 3", OperationSystem = "Android"}
            };

            var mockPhoneRepository = new Mock<IPhoneRepository>();
            mockPhoneRepository.Setup(p => p.GetAllPhones()).Returns(phones);

            var controller = new ShopController(null, mockPhoneRepository.Object, null);

            //Act
            var result = controller.Index("Android");

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Phone>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }



        //Details
        [Fact]
        public void Details_ReturnsViewResult_WithPhone()
        {
            //Arrange 
            int phoneId = 1;
            var phone = new Phone
            {
                PhoneId = phoneId,
                Name = "Test Name",
                OperationSystem = "Test OS",
                Picture = "Test Picture",
                Description = "Test Description",
                Price = 1000,
                Quantity = 2
            };

            var mockPhoneRepository = new Mock<IPhoneRepository>();
            mockPhoneRepository.Setup(c => c.GetPhoneById(phoneId)).Returns(phone);

            var controller = new ShopController(null, mockPhoneRepository.Object, null);

            //Act
            var result = controller.Details(phoneId);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Phone>(viewResult.ViewData.Model);
            Assert.Equal(phone, model);
        }

        [Fact]
        public void Details_ReturnsNotFound_WhenPhoneNotFound()
        {
            // Arrange
            int phoneId = 1;

            var mockPhoneRepository = new Mock<IPhoneRepository>();
            mockPhoneRepository.Setup(repo => repo.GetPhoneById(phoneId)).Returns((Phone)null);

            var controller = new ShopController(null, mockPhoneRepository.Object, null);

            // Act
            var result = controller.Details(phoneId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
