using ChatCase.Domain.Dto.Request.Identity;
using FluentValidation;

namespace ChatCase.Domain.Validation.Identity
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(m => m.FirstName)
             .NotEmpty().WithMessage(ValidationMessage.Required);

            RuleFor(m => m.LastName)
              .NotEmpty().WithMessage(ValidationMessage.Required);

            RuleFor(m => m.Password)
              .NotEmpty().WithMessage(ValidationMessage.Required);

            RuleFor(m => m.RePassword)
             .NotEmpty().WithMessage(ValidationMessage.Required);

            RuleFor(m => m.Email)
            .NotEmpty().WithMessage(ValidationMessage.Required);
        }
    }
}
