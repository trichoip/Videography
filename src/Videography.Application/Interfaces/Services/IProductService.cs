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

    //Task<IList<ProductResponse>> GetProductsForCategoryAsync(int categoryId);
    //Task<IList<ProductResponse>> GetProductsForBookingAsync(int bookingId);
    //Task<IList<ProductResponse>> GetProductsInWishlistAsync(int userId);
    //Task AddToWishlistAsync(User user, int productId); // them vao danh sach yeu thich
    //Task RemoveFromWishlistAsync(User user, int productId); // xoa khoi danh sach yeu thich

    Task<bool> IsInWishlistAsync(int productId);
    Task<bool> HasCategoryAsync(int categoryId);

    Task<IList<ImageResponse>> GetImagesAsync(int productId);
    Task AddImageAsync(int productId, IFormFile image);
    Task RemoveImageAsync(int productId, int imageId);
    Task AddImagesAsync(int productId, IFormFileCollection images);
    Task RemoveImagesAsync(int productId);
    //Task RemoveImagesAsync(IEnumerable<int> imageIds);

    Task<PaginatedList<ReviewResponse>> GetReviewsAsync(int productId, int pageIndex, int pageSize);

    Task<IList<BookingItemValidResponse>> GetValidBookingItemsAsync(int productId);
}
