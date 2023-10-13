using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.Bookings;
public class CreateBookingRequest : IMapFrom<Booking>
{
    public int AddressId { get; set; }
    public int CreditCardId { get; set; }
}
