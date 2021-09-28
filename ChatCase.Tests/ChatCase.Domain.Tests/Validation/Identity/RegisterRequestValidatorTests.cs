using ChatCase.Domain.Dto.Request.Identity;
using ChatCase.Domain.Validation.Identity;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace ChatCase.Tests.ChatCase.Domain.Tests.Validation.Identity
{
    [TestFixture]
    public class RegisterRequestValidatorTests : BaseTest
    {
        private RegisterRequestValidator _registerRequestValidator;

        [OneTimeSetUp]
        public void SetUp()
        {
            _registerRequestValidator = GetService<RegisterRequestValidator>();
        }

        [Test]
        public void ShouldHaveErrorWhenFirstNameIsNullOrEmpty()
        {
            var model = new RegisterRequest
            {
                FirstName = null
            };

            _registerRequestValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);

            model.FirstName = string.Empty;
            _registerRequestValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Test]
        public void ShouldHaveErrorWhenLastNameIsNullOrEmpty()
        {
            var model = new RegisterRequest
            {
                LastName = null
            };

            _registerRequestValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);

            model.LastName = string.Empty;
            _registerRequestValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Test]
        public void ShouldHaveErrorWhenPasswordIsNullOrEmpty()
        {
            var model = new RegisterRequest
            {
                Password = null
            };

            _registerRequestValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Password);

            model.Password = string.Empty;
            _registerRequestValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
        {
            var model = new RegisterRequest
            {
                Email = null
            };

            _registerRequestValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);

            model.Email = string.Empty;
            _registerRequestValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        }
    }
}
