using Videography.Application.DTOs.Categories;

namespace Videography.Application.Interfaces.Services;
public interface ICategoryService
{
    Task<IList<CategoryResponse>> GetCategoriesAsync();
    Task<CategoryResponse?> FindByIdAsync(int id);
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);
    Task<CategoryResponse> UpdateAsync(UpdateCategoryRequest request);
    Task DeleteAsync(int id);
}
