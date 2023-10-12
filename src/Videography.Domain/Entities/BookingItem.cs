using Videography.Domain.Common;

namespace Videography.Domain.Entities;
public class BookingItem : BaseEntity
{
    public int Quantity { get; set; }
    public decimal Amount { get; set; }

    public bool IsReviewed { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public int BookingId { get; set; }
    public int ProductId { get; set; }
    public virtual Review Review { get; set; } = default!;
    public virtual Booking Booking { get; set; } = default!;
    public virtual Product Product { get; set; } = default!;

}
