using ChatCase.Business.Interfaces.Identity;
using ChatCase.Domain.Dto.Request.Identity;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ChatCase.Tests.ChatCase.Services.Tests.Identity
{
    [TestFixture]
    public class UserServiceTests : ServiceTest
    {
        private IUserService _userService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _userService = GetService<IUserService>();
        }

        [Test]
        public void ItShouldReturnNullUserWhenEmailIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _userService.GetByEmailAsync(null));
        }

        [Test]
        public void ItShouldReturnNullUserWhenUserNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _userService.GetByUserNameAsync(null));
        }

        [Test]
        public void ItShouldThrowIfRegisterRequestIsNullWhenInsertUser()
        {
            Assert.Throws<ArgumentNullException>(() => _userService.RegisterAsync(null));
        }

        [Test]
        public async Task ItShouldReturnAnErrorWhenPasswordsDoNotMatch()
        {
            RegisterRequest request = new();

            request.Password = "example";
            request.Password = "example2";

            var serviceResponse = await _userService.RegisterAsync(request);
            serviceResponse.Data.Should().NotBeNull();
        }
    }
}
