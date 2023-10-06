using Videography.Domain.Common;

namespace Videography.Domain.Entities;
public class CreditCard : BaseEntity
{
    public string CardNumber { get; set; } = default!;
    public string CardHolderName { get; set; } = default!;
    public string CVV { get; set; } = default!;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public bool IsPrimary { get; set; }
    public int CreditCardTypeId { get; set; }
    public int UserId { get; set; }
    public virtual CreditCardType CreditCardType { get; set; } = default!;
    public virtual User User { get; set; } = default!;

    public virtual ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
}
