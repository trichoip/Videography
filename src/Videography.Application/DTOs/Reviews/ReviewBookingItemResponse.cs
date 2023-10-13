using Videography.Application.Common.Mappings;
using Videography.Domain.Common;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.Reviews;
public class ReviewBookingItemResponse : BaseEntity, IMapFrom<Review>
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public double Rating { get; set; }
    public int bookingItemId { get; set; }
}
