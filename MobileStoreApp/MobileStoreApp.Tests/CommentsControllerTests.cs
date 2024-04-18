using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class CommentsControllerTests
    {
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfComments()
        {
            // Arrange
            var mockDbContex = new Mock<ApplicationDbContext>();
            var mockDbSet = new Mock<DbSet<Comment>>();
            var mockUserManager = MockUserManager<ApplicationUser>();

            var comments = new List<Comment>
            {
                new Comment { CommnetId = 1, Content = "Content 1" },
                new Comment { CommnetId = 2, Content = "Content 2" }
            }.AsQueryable();

            //var mockDbSet = new Mock<DbSet<Comment>>();
            mockDbSet.As<IQueryable<Comment>>().Setup(m => m.Provider).Returns(comments.Provider);
            mockDbSet.As<IQueryable<Comment>>().Setup(m => m.Expression).Returns(comments.Expression);
            mockDbSet.As<IQueryable<Comment>>().Setup(m => m.ElementType).Returns(comments.ElementType);
            mockDbSet.As<IQueryable<Comment>>().Setup(m => m.GetEnumerator()).Returns(comments.GetEnumerator());
           
            mockDbContex.Setup(c => c.Set<Comment>()).Returns(mockDbSet.Object); // Mocking Set<Comment>() method

            //mockDbContex.Setup(c => c.Comments).Returns(mockDbSet.Object);

            var controller = new CommentsController(mockDbContex.Object, mockUserManager.Object);

            //Act
            var result = await controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Comment>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        private static Mock<UserManager<ApplicationUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
        }
    }
}
