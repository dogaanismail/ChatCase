using ChatCase.Domain.Dto.Request.Identity;
using FluentValidation;

namespace ChatCase.Domain.Validation.Identity
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(m => m.UserName)
                .NotEmpty().WithMessage(ValidationMessage.Required);

            RuleFor(m => m.Password)
                .NotEmpty().WithMessage(ValidationMessage.Required);
        }
    }
}
