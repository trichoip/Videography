using FluentValidation;

namespace Videography.Application.DTOs.Reviews;
public class UpdateReviewRequestValidator : AbstractValidator<UpdateReviewRequest>
{
    public UpdateReviewRequestValidator()
    {
        RuleFor(x => x.Rating).NotEmpty().InclusiveBetween(1, 5);
    }
}
