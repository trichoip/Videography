using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.CreditCardTypes;
public class CreateCreditCardTypeRequest : IMapFrom<CreditCardType>
{
    public string Name { get; set; } = default!;
}
