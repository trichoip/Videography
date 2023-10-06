using Videography.Domain.Common;

namespace Videography.Application.DTOs;
public class ProductDto : BaseEntity
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public int UnitsInStock { get; set; }
    public int TotalReviews { get; set; }
    public double AverageRating { get; set; }
    public int CategoryId { get; set; }
}
