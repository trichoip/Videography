using FluentValidation;

namespace Videography.Application.DTOs.Carts;
public class CreateCartItemRequestValidator : AbstractValidator<CreateCartItemRequest>
{
    public CreateCartItemRequestValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be greater than start date")
            .GreaterThanOrEqualTo(x => DateOnly.FromDateTime(DateTime.Now)).WithMessage("End date must be greater than today")
            .WithMessage("End date must be greater than today");

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(x => DateOnly.FromDateTime(DateTime.Now)).WithMessage("Start date must be greater than today")
            .LessThanOrEqualTo(x => x.EndDate).WithMessage("Start date must be less than end date");
    }
}

