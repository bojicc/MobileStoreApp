using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileStoreApp.Controllers;
using MobileStoreApp.Data;
using MobileStoreApp.Data.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace MobileStoreApp.Tests
{
    public class PhonesControllerTests
    {
        //Index 
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfPhones()
        {
            //Arrage 
            var mockDbSet = new Mock<DbSet<Phone>>();
            var phones = new List<Phone>
            {
                new Phone { PhoneId = 1, Name = "Phone 1", OperationSystem = "Android", Picture = "Picture 1", Description = "Description 1", Price = 1000, Quantity = 4},
                new Phone { PhoneId = 2, Name = "Phone 2", OperationSystem = "Android", Picture = "Picture 2", Description = "Description 2", Price = 1300, Quantity = 1},
                new Phone { PhoneId = 3, Name = "Phone 3", OperationSystem = "Android", Picture = "Picture 3", Description = "Description 3", Price = 2000, Quantity = 7}
            }.AsQueryable();

            mockDbSet.As<IAsyncEnumerable<Phone>>()
            .Setup(m => m.GetAsyncEnumerator(CancellationToken.None))
            .Returns(new TestAsyncEnumerator<Phone>(phones.GetEnumerator()));


            mockDbSet.As<IQueryable<Phone>>().Setup(m => m.Provider).Returns(phones.Provider);
            mockDbSet.As<IQueryable<Phone>>().Setup(m => m.Expression).Returns(phones.Expression);
            mockDbSet.As<IQueryable<Phone>>().Setup(m => m.ElementType).Returns(phones.ElementType);
            mockDbSet.As<IQueryable<Phone>>().Setup(m => m.GetEnumerator()).Returns(phones.GetEnumerator());

            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(c => c.Phones).Returns(mockDbSet.Object);

            var controller = new PhonesController(mockContext.Object);

            //Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Phone>>(viewResult.ViewData.Model);
            Assert.Equal(3, model.Count()); // Provjerava da li se vraća lista od tri telefona
        }



        //Details
        [Fact]
        public async Task Details_ReturnsViewResult_WithCorrectPhone()
        {
            //Arrange
            var phoneId = 1;
            var phone = new Phone
            {
                PhoneId = phoneId,
                Name = "Test Phone",
                OperationSystem = "Android",
                Picture = "Test Picture",
                Description = "Test Description",
                Price = 1000,
                Quantity = 4
            };

            var phones = new List<Phone> { phone }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Phone>>();
            mockDbSet.As<IQueryable<Phone>>().Setup(m => m.Provider).Returns(phones.Provider);
            mockDbSet.As<IQueryable<Phone>>().Setup(m => m.Expression).Returns(phones.Expression);
            mockDbSet.As<IQueryable<Phone>>().Setup(m => m.ElementType).Returns(phones.ElementType);
            mockDbSet.As<IQueryable<Phone>>().Setup(m => m.GetEnumerator()).Returns(phones.GetEnumerator());

            mockDbSet.Setup(x => x.FindAsync(phoneId)).ReturnsAsync(phone);


            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(c => c.Phones).Returns(mockDbSet.Object);

            var controller = new PhonesController(mockContext.Object);

            //Act
            var result = await controller.Details(phoneId);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Phone>(viewResult.ViewData.Model);
            Assert.Equal(phone, model);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? phoneId = null;

            var mockContext = new Mock<IApplicationDbContext>();
            var controller = new PhonesController(mockContext.Object);

            // Act
            var result = await controller.Details(phoneId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenPhoneNotFound()
        {
            //Arrange
            int phoneId = 1;

            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(c => c.Phones.FindAsync(phoneId)).ReturnsAsync((Phone)null);

            var controller = new PhonesController(mockContext.Object);

            //Act
            var result = await controller.Details(phoneId);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }



        //Create
        [Fact]
        public async Task Create_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            //Arrange 
            var phone = new Phone
            {
                PhoneId = 1,
                Name = "Test Phone",
                Description = "Test Description",
                Price = 1000,
                Quantity = 4,
                Picture = "Test Picture",
                OperationSystem = "Test OS"
            };

            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(c => c.Add(phone)).Verifiable();
            mockContext.Setup(c => c.SaveChangesAsync(default)).Returns(Task.FromResult(0)).Verifiable();

            var controller = new PhonesController(mockContext.Object);

            //Act
            var result = await controller.Create(phone);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockContext.Verify();
        }

        [Fact]
        public async Task Create_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            //Arrange
            var phone = new Phone();

            var mockContext = new Mock<IApplicationDbContext>();

            var controller = new PhonesController(mockContext.Object);
            controller.ModelState.AddModelError("Name", "Name is required");

            //Act
            var result = await controller.Create(phone);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(phone, viewResult.ViewData.Model);

        }



        //Edit
        [Fact]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            //Arrange
            int? phoneId = null;

            var mockContext = new Mock<IApplicationDbContext>();
            var controller = new PhonesController(mockContext.Object);

            //Act
            var result = await controller.Edit(phoneId);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenPhoneNotFound()
        {
            //Arrange
            int phoneId = 1;

            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(c => c.Phones.FindAsync(phoneId)).ReturnsAsync((Phone)null);

            var controller = new PhonesController(mockContext.Object);

            //Act
            var result = await controller.Edit(phoneId);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task Edit_ReturnsViewResult_WithPhoneModel()
        {
            int phoneId = 1;
            var phone = new Phone
            {
                PhoneId = phoneId,
                Name = "Test Phone",
                Description = "Test Description",
                Price = 1000,
                Quantity = 4,
                Picture = "Test Picture",
                OperationSystem = "Test OS"
            };

            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(c => c.Phones.FindAsync(phoneId)).ReturnsAsync(phone);

            var controller = new PhonesController(mockContext.Object);

            //Act
            var result = await controller.Edit(phoneId);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Phone>(viewResult.ViewData.Model);
            Assert.Equal(phone, model);
        }

        [Fact]
        public async Task Edit_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            int phoneId = 1;
            var phone = new Phone
            {
                PhoneId = phoneId,
                Name = "Test Phone",
                Description = "Test Description",
                Price = 1000,
                Quantity = 4,
                Picture = "Test Picture",
                OperationSystem = "Test OS"
            };

            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(c => c.Update(phone)).Verifiable();
            mockContext.Setup(c => c.SaveChangesAsync(default)).Returns(Task.FromResult(0)).Verifiable();

            var controller = new PhonesController(mockContext.Object);

            //Act
            var result = await controller.Edit(phoneId, phone);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockContext.Verify();
        }



        //Delete
        [Fact]
        public async Task Delete_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            var mockContext = new Mock<ApplicationDbContext>();
            var controller = new PhonesController(mockContext.Object);

            // Act
            var result = await controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
