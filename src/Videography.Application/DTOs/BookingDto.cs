using System.ComponentModel.DataAnnotations;
using Videography.Application.DTOs.Addresses;
using Videography.Application.DTOs.CreditCards;
using Videography.Application.DTOs.Users;
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

    public virtual UserResponse User { get; set; } = default!;
    public virtual AddressResponse Address { get; set; } = default!;
    public virtual CreditCardResponse CreditCard { get; set; } = default!;

    public virtual ICollection<BookingItemDto> BookingItems { get; set; } = new HashSet<BookingItemDto>();
}
