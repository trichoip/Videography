using System.ComponentModel.DataAnnotations;
using Videography.Domain.Common;
using Videography.Domain.Enums;

namespace Videography.Domain.Entities;
public class Booking : BaseEntity
{
    public decimal TotalAmount { get; set; }
    public int TotalQuantity { get; set; }

    [EnumDataType(typeof(BookStatus))]
    public BookStatus Status { get; set; }

    public int UserId { get; set; }
    public int? AddressId { get; set; }
    public int? CreditCardId { get; set; }

    public virtual User User { get; set; } = default!;
    public virtual Address? Address { get; set; }
    public virtual CreditCard? CreditCard { get; set; }

    public virtual ICollection<BookingItem> BookingItems { get; set; } = new HashSet<BookingItem>();
}
