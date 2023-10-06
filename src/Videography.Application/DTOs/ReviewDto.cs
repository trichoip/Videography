using Videography.Domain.Common;

namespace Videography.Application.DTOs;
public class ReviewDto : BaseEntity
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public double Rating { get; set; }
}
