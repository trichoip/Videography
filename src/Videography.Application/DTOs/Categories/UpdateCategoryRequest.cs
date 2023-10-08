using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.Categories;
public class UpdateCategoryRequest : IMapFrom<Category>
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}
