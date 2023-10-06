using FluentValidation;
using Videography.Application.DTOs;

namespace Videography.Application.Validators;
public class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name is required").MinimumLength(10);
        RuleFor(x => x.Description).NotNull().NotEmpty().WithMessage("Name is required").MinimumLength(10);
    }
}
