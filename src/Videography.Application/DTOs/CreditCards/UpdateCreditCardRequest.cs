using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.CreditCards;
public class UpdateCreditCardRequest : IMapFrom<CreditCard>
{
    public int Id { get; set; }
    public string CardNumber { get; set; } = default!;
    public string CardHolderName { get; set; } = default!;
    public string CVV { get; set; } = default!;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public bool IsPrimary { get; set; }
    public int CreditCardTypeId { get; set; }
}
