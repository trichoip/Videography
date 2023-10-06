using Videography.Domain.Common;

namespace Videography.Application.DTOs;
public class CreditCardDto : BaseEntity
{
    public string? CardNumber { get; set; }
    public string? CardHolderName { get; set; }
    public string? ExpirationDate { get; set; }
    public string? CVV { get; set; }

    public int CreditCardTypeId { get; set; }
    public int UserId { get; set; }
    public virtual CreditCardTypeDto CreditCardType { get; set; } = default!;
    public virtual UserDto User { get; set; } = default!;

    public virtual ICollection<BookingDto> Bookings { get; set; } = new HashSet<BookingDto>();
}
