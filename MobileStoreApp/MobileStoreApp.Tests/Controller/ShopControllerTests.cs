using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileStoreApp.Controllers;
using MobileStoreApp.Data;
using MobileStoreApp.Data.Models;
using MobileStoreApp.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileStoreApp.Tests.Controller
{
    public class ShopControllerTests
    {
        private ShopController _shopController;

        private ApplicationDbContext _context;
        private IPhoneRepository _phoneRepository;
        private UserManager<ApplicationUser> _userManager;

        public ShopControllerTests() 
        {
            //Depenedencies
            _context = A.Fake<ApplicationDbContext>();
            _phoneRepository = A.Fake<IPhoneRepository>();
            _userManager = A.Fake<UserManager<ApplicationUser>>();

            //SUT
            _shopController = new ShopController(_context, _phoneRepository, null);
        }

        [Fact]
        public void ShopController_Index_ReturnsSuccess()
        {
            //Arange - what do I need to bring in?
            var phones = A.Fake<DbSet<Phone>>();
            A.CallTo(() => _phoneRepository.GetAllPhones()).Returns(phones);

            //Act
            //var result = _shopController.Index();

            //Assert - object check actions
            //result.Should().BeOfType<Task<IActionResult>>();

        }
    }
}
