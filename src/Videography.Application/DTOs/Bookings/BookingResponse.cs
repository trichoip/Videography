using Videography.Application.Common.Mappings;
using Videography.Domain.Common;
using Videography.Domain.Entities;
using Videography.Domain.Enums;

namespace Videography.Application.DTOs;
public class BookingResponse : BaseEntity, IMapFrom<Booking>
{
    public decimal TotalAmount { get; set; }
    public int TotalQuantity { get; set; }
    public BookingStatus Status { get; set; }
    public int AddressId { get; set; }
    public int CreditCardId { get; set; }
}
