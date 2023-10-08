using FluentValidation;

namespace Videography.Application.DTOs.Categories;
public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
