using ChatCase.Domain.Dto.Request.Identity;
using ChatCase.Domain.Validation.Identity;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace ChatCase.Tests.ChatCase.Domain.Tests.Validation.Identity
{
    [TestFixture]
    public class LoginRequestValidatorTests : BaseTest
    {
        private LoginRequestValidator _loginRequestValidator;

        [OneTimeSetUp]
        public void SetUp()
        {
            _loginRequestValidator = GetService<LoginRequestValidator>();
        }

        [Test]
        public void ShouldHaveErrorWhenUserNameIsNullOrEmpty()
        {
            var model = new LoginRequest
            {
                UserName = null
            };

            _loginRequestValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.UserName);

            model.UserName = string.Empty;
            _loginRequestValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.UserName);
        }

        [Test]
        public void ShouldHaveErrorWhenPasswordIsNullOrEmpty()
        {
            var model = new LoginRequest
            {
                Password = null
            };

            _loginRequestValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Password);

            model.Password = string.Empty;
            _loginRequestValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
}
