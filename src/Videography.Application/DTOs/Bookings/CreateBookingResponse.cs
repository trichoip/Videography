using Videography.Application.Common.Mappings;
using Videography.Application.DTOs.BookingItems;
using Videography.Domain.Common;
using Videography.Domain.Entities;
using Videography.Domain.Enums;

namespace Videography.Application.DTOs.Bookings;
public class CreateBookingResponse : BaseEntity, IMapFrom<Booking>
{
    public decimal TotalAmount { get; set; }
    public int TotalQuantity { get; set; }
    public BookingStatus Status { get; set; }
    public int AddressId { get; set; }
    public int CreditCardId { get; set; }
    public ICollection<BookingItemResponse> BookingItems { get; set; } = new HashSet<BookingItemResponse>();
}
