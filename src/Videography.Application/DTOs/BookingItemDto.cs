using Videography.Domain.Common;

namespace Videography.Application.DTOs;
public class BookingItemDto : BaseEntity
{
    public int Quantity { get; set; }
    public decimal Amount { get; set; }

    public bool IsReview { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int BookingId { get; set; }
    public int ProductId { get; set; }

    public virtual BookingDto Booking { get; set; } = default!;
    public virtual ProductDto Product { get; set; } = default!;

}
