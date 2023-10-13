using FluentValidation;

namespace Videography.Application.DTOs.Reviews;
public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
{
    public CreateReviewRequestValidator()
    {
        RuleFor(x => x.Rating).NotEmpty().InclusiveBetween(1, 5);
    }
}

