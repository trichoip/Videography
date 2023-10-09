using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using Videography.Application.Common.Exceptions;
using Videography.Application.Common.Mappings;
using Videography.Application.DTOs.Images;
using Videography.Application.DTOs.Products;
using Videography.Application.Extensions;
using Videography.Application.Helpers;
using Videography.Application.Interfaces.Repositories;
using Videography.Application.Interfaces.Services;
using Videography.Domain.Entities;

namespace Videography.Infrastructure.Services;
public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;
    public ProductService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        UserManager<User> userManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
    {
        if (!await IsHasCategoryAsync(request.CategoryId))
            throw new NotFoundException(nameof(Category), request.CategoryId);

        var product = _mapper.Map<Product>(request);
        await _unitOfWork.ProductRepository.CreateAsync(product);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<ProductResponse>(product);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _unitOfWork.ProductRepository.FindByIdAsync(id);
        if (product is null) throw new NotFoundException(nameof(Product), id);
        await _unitOfWork.ProductRepository.DeleteAsync(product);
        await _unitOfWork.CommitAsync();
    }
    public async Task<ProductResponse> UpdateAsync(UpdateProductRequest request)
    {
        if (!await IsHasCategoryAsync(request.CategoryId))
            throw new NotFoundException(nameof(Category), request.CategoryId);

        var product = await _unitOfWork.ProductRepository.FindByIdAsync(request.Id);
        if (product is null) throw new NotFoundException(nameof(Product), request.Id);

        _mapper.Map(request, product);
        //await _unitOfWork.ProductRepository.UpdateAsync(product);

        await _unitOfWork.CommitAsync();
        return _mapper.Map<ProductResponse>(product);
    }

    public async Task<ProductResponse?> FindByIdAsync(int id)
    {
        var product = await _unitOfWork.ProductRepository.FindByIdAsync(id);
        if (product is null) throw new NotFoundException(nameof(Product), id);

        var productResponse = _mapper.Map<ProductResponse>(product);
        productResponse.IsInWislish = await IsInWishlistAsync(product.Id);

        return productResponse;

    }
    public async Task<PaginatedList<ProductResponse>> GetProductsAsync(
        int pageIndex = 0,
        int pageSize = 0,
        Expression<Func<Product, bool>>? expression = null,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
        Func<IQueryable<Product>, IQueryable<Product>>? includeFunc = null)
    {

        var productIQ = await _unitOfWork.ProductRepository.FindToIQueryableAsync(expression, orderBy, includeFunc);
        var paginationProducts = await _mapper.ProjectTo<ProductResponse>(productIQ).PaginatedListAsync(pageIndex, pageSize);

        paginationProducts.ForEach(product => product.IsInWislish = IsInWishlistAsync(product.Id).Result);

        return paginationProducts;
    }

    public async Task<bool> IsInWishlistAsync(int productId)
    {
        if (_httpContextAccessor.HttpContext?.User is not { } userClaimsPrincipal) return false;

        var user = await _userManager.GetUserAsync(userClaimsPrincipal);
        if (user == null) return false;

        //string? userId = _httpContextAccessor.HttpContext?.userClaimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier);
        //if (userId == null) return false;

        return await _unitOfWork.WishlistRepository
                  .ExistsByAsync(c => c.UserId == user.Id && c.ProductId == productId);
    }

    public async Task<bool> IsHasCategoryAsync(int categoryId)
    {
        return await _unitOfWork.CategoryRepository.ExistsByAsync(c => c.Id == categoryId);
    }

    public async Task<IList<ImageResponse>> GetImagesAsync(int productId)
    {
        if (!await _unitOfWork.ProductRepository.ExistsByAsync(c => c.Id == productId))
            throw new NotFoundException(nameof(Product), productId);
        var images = await _unitOfWork.ImageRepository.FindAsync(c => c.ProductId == productId);
        //if (images.IsNullOrEmpty()) throw new NotFoundException($"product {productId} not have any image");
        return _mapper.Map<IList<ImageResponse>>(images);
    }

    public async Task AddImagesAsync(int productId, IFormFileCollection images)
    {
        var product = await _unitOfWork.ProductRepository.FindByIdAsync(productId);

        if (product is null) throw new NotFoundException(nameof(Product), productId);

        foreach (var image in images)
        {
            var imageEntity = new Image
            {
                Product = product
            };

            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                imageEntity.Data = ms.ToArray();
            }

            await _unitOfWork.ImageRepository.CreateAsync(imageEntity);
        }

        await _unitOfWork.CommitAsync();
    }

    public async Task AddImageAsync(int productId, IFormFile image)
    {
        var product = await _unitOfWork.ProductRepository.FindByIdAsync(productId);

        if (product is null) throw new NotFoundException(nameof(Product), productId);

        var imageEntity = new Image
        {
            Product = product
        };

        using (var ms = new MemoryStream())
        {
            await image.CopyToAsync(ms);
            imageEntity.Data = ms.ToArray();
        }

        await _unitOfWork.ImageRepository.CreateAsync(imageEntity);
        await _unitOfWork.CommitAsync();
    }

    public async Task RemoveImageAsync(int productId, int imageId)
    {
        if (!await _unitOfWork.ProductRepository.ExistsByAsync(c => c.Id == productId))
            throw new NotFoundException(nameof(Product), productId);
        var image = await _unitOfWork.ImageRepository.FindByAsync(c => c.ProductId == productId && c.Id == imageId);
        if (image is null) throw new NotFoundException($"Product {productId} not have image {imageId}");
        await _unitOfWork.ImageRepository.DeleteAsync(image);
        await _unitOfWork.CommitAsync();

    }

    public async Task RemoveImagesAsync(int productId)
    {
        if (!await _unitOfWork.ProductRepository.ExistsByAsync(c => c.Id == productId))
            throw new NotFoundException(nameof(Product), productId);
        var images = await _unitOfWork.ImageRepository.FindAsync(c => c.ProductId == productId);
        if (images.IsNullOrEmpty()) throw new NotFoundException($"product {productId} not have any image");
        await _unitOfWork.ImageRepository.DeleteRangeAsync(images);
        await _unitOfWork.CommitAsync();
    }
}
