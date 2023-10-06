using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs;
public class CreditCardTypeDto : IMapFrom<CreditCardType>
{
    public int Id { get; set; }
    public string? Name { get; set; }
}
