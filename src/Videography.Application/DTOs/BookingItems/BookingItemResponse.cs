using Videography.Application.Common.Mappings;
using Videography.Domain.Common;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.BookingItems;
public class BookingItemResponse : BaseEntity, IMapFrom<BookingItem>
{
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public bool IsReviewed { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int BookingId { get; set; }
    public int ProductId { get; set; }
}
