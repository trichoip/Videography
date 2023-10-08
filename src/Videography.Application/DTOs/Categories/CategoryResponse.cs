using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.Categories;

public class CategoryResponse : IMapFrom<Category>
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}
