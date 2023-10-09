using Videography.Application.Common.Mappings;
using Videography.Application.DTOs.Categories;
using Videography.Application.DTOs.Images;
using Videography.Domain.Common;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.Products;
public class ProductResponse : BaseEntity, IMapFrom<Product>
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Amount { get; set; }
    public int UnitsInStock { get; set; }
    public int TotalReviews { get; set; }
    public double AverageRating { get; set; }
    public bool IsInWislish { get; set; }

    public int CategoryId { get; set; }
    public CategoryResponse Category { get; set; } = default!;

    public ICollection<ImageResponse> Images { get; set; } = new HashSet<ImageResponse>();

}
