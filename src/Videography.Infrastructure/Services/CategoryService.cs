using AutoMapper;
using Videography.Application.Common.Exceptions;
using Videography.Application.DTOs.Categories;
using Videography.Application.Interfaces.Repositories;
using Videography.Application.Interfaces.Services;
using Videography.Domain.Entities;

namespace Videography.Infrastructure.Services;
public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        var category = _mapper.Map<Category>(request);
        await _unitOfWork.CategoryRepository.CreateAsync(category);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _unitOfWork.CategoryRepository.FindByIdAsync(id);
        if (category == null)
        {
            throw new NotFoundException(nameof(Category), id);
        }
        await _unitOfWork.CategoryRepository.DeleteAsync(category);
        await _unitOfWork.CommitAsync();
    }

    public async Task<CategoryResponse?> FindByIdAsync(int id)
    {
        var category = await _unitOfWork.CategoryRepository.FindByIdAsync(id);
        if (category == null)
        {
            throw new NotFoundException(nameof(Category), id);
        }
        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task<IList<CategoryResponse>> GetCategoriesAsync()
    {
        var categories = await _unitOfWork.CategoryRepository.FindAsync();
        return _mapper.Map<IList<CategoryResponse>>(categories);
    }

    public async Task<CategoryResponse> UpdateAsync(UpdateCategoryRequest request)
    {
        var category = await _unitOfWork.CategoryRepository.FindByIdAsync(request.Id);
        if (category == null)
        {
            throw new NotFoundException(nameof(Category), request.Id);
        }

        _mapper.Map(request, category);
        // nếu sài UpdateAsync thì lúc nào cũng update toàn bộ các preperty kể cả nó không thay đổi
        // còn nếu không sài thì UpdateAsync thì chỉ update các preperty thay đổi, các preperty không thay đổi sẽ không viết vào query,  lưu ý: là không được có AsNotracking() 
        // nếu có AsNotracking thì phải sài UpdateAsync mới update được
        //await _unitOfWork.CategoryRepository.UpdateAsync(category);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<CategoryResponse>(category);
    }
}
