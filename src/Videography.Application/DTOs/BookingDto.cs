using System.ComponentModel.DataAnnotations;
using Videography.Domain.Common;
using Videography.Domain.Enums;

namespace Videography.Application.DTOs;
public class BookingDto : BaseEntity
{
    public decimal TotalAmount { get; set; }
    public int TotalQuantity { get; set; }

    [EnumDataType(typeof(BookStatus))]
    public BookStatus BookStatus { get; set; }

    public int AddressId { get; set; }
    public int UserId { get; set; }
    public int CreditCardId { get; set; }

    public virtual UserDto User { get; set; } = default!;
    public virtual AddressDto Address { get; set; } = default!;
    public virtual CreditCardDto CreditCard { get; set; } = default!;

    public virtual ICollection<BookingItemDto> BookingItems { get; set; } = new HashSet<BookingItemDto>();
}
