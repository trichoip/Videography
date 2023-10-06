using Videography.Domain.Common;

namespace Videography.Domain.Entities;
public class Review : BaseEntity
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public double Rating { get; set; }
    public int bookingItemId { get; set; }

    public virtual BookingItem BookingItem { get; set; } = default!;
}
