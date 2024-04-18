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

            var mockDbSet = new Mock<DbSet<Phone>>();
            mockDbSet.Setup(p => p.FindAsync(phoneId)).ReturnsAsync(phone);

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
            var controller = new PhonesController(Mock.Of<ApplicationDbContext>());

            // Act
            var result = await controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }


    }
}
