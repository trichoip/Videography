using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.Products;
public class CreateProductRequest : IMapFrom<Product>
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Amount { get; set; }
    public int UnitsInStock { get; set; }
    public int CategoryId { get; set; }

}
