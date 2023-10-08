using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.Categories;
public class CreateCategoryRequest : IMapFrom<Category>
{
    public string Name { get; set; } = default!;
}
