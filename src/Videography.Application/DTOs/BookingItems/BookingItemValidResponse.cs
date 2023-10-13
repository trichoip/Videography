using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.BookingItems;
public class BookingItemValidResponse : IMapFrom<BookingItem>
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
