using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using Videography.Application.DTOs.BookingItems;
using Videography.Application.DTOs.Images;
using Videography.Application.DTOs.Products;
using Videography.Application.DTOs.Reviews;
using Videography.Application.Helpers;
using Videography.Domain.Entities;

namespace Videography.Application.Interfaces.Services;
public interface IProductService
{
    Task<ProductResponse?> FindByIdAsync(int id);
    Task<ProductResponse> CreateAsync(CreateProductRequest request);
    Task<ProductResponse> UpdateAsync(UpdateProductRequest request);
    Task DeleteAsync(int id);
    Task<PaginatedList<ProductResponse>> GetProductsAsync(
            int pageIndex = 0,
            int pageSize = 0,
            Expression<Func<Product, bool>>? expression = null,
            Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
            Func<IQueryable<Product>, IQueryable<Product>>? includeFunc = null);

    Task<bool> IsInWishlistAsync(int productId);
    Task<bool> IsHasCategoryAsync(int categoryId);

    Task<IList<ImageResponse>> GetImagesAsync(int productId);
    Task AddImageAsync(int productId, IFormFile image);
    Task RemoveImageAsync(int productId, int imageId);
    Task AddImagesAsync(int productId, IFormFileCollection images);
    Task RemoveImagesAsync(int productId);

    Task<PaginatedList<ReviewResponse>> GetReviewsAsync(int productId, int pageIndex, int pageSize);

    Task<IList<BookingItemValidResponse>> FindValidBookingItemsAsync(int productId);
}
