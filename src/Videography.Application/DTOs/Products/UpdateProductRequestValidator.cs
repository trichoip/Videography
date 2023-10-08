using FluentValidation;

namespace Videography.Application.DTOs.Products;
public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {

        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();

    }
}
