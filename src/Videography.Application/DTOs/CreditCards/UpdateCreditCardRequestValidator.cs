using FluentValidation;

namespace Videography.Application.DTOs.CreditCards;
public class UpdateCreditCardRequestValidator : AbstractValidator<UpdateCreditCardRequest>
{
    public UpdateCreditCardRequestValidator()
    {

        RuleFor(x => x.CardNumber).NotEmpty().CreditCard();
        RuleFor(x => x.CardHolderName).NotEmpty();
        RuleFor(x => x.CVV).NotEmpty().Length(4);
        RuleFor(x => x.ExpiryMonth).NotEmpty().InclusiveBetween(1, 12);
        RuleFor(x => x.ExpiryYear).NotEmpty().InclusiveBetween(DateTime.UtcNow.Year, DateTime.UtcNow.Year + 10);
        RuleFor(x => x.IsPrimary).NotNull();

    }
}
