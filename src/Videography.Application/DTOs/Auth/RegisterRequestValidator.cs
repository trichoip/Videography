using FluentValidation;

namespace Videography.Application.DTOs.Auth;
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        RuleFor(x => x.Email).EmailAddress().NotEmpty().WithMessage("Email is required");
    }
}
