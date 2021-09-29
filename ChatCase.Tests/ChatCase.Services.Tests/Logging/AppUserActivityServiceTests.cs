using ChatCase.Business.Interfaces.Logging;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ChatCase.Tests.ChatCase.Services.Tests.Logging
{
    [TestFixture]
    public class AppUserActivityServiceTests : ServiceTest
    {
        private IAppUserActivityService _userActivityService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _userActivityService = GetService<IAppUserActivityService>();
        }

        [Test]
        public void ItShouldReturnErrorWhenEmailIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _userActivityService.GetActivitiesByEmail(null));
        }

        [Test]
        public void ItShouldReturnErrorWhenUserNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _userActivityService.GetActivitiesByUserName(null));
        }

        [Test]
        public async Task ItShouldInsert()
        {
            var exisitingLog = await _userActivityService.InsertAsync("34343", "deneme", "deneme");
            exisitingLog.Should().NotBeNull();
        }
    }
}
