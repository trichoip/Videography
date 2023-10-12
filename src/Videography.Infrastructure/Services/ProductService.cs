using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Videography.Application.Common.Exceptions;
using Videography.Application.Common.Mappings;
using Videography.Application.DTOs.Images;
using Videography.Application.DTOs.Products;
using Videography.Application.DTOs.Reviews;
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
    private readonly IUrlHelper _urlHelper;
    private readonly LinkGenerator _generator;

    public ProductService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        UserManager<User> userManager,
        IHttpContextAccessor httpContextAccessor,
        IUrlHelper urlHelper,
        LinkGenerator generator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _urlHelper = urlHelper;
        _generator = generator;
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
        //var product = await _unitOfWork.ProductRepository.FindByIdAsync(id);
        //if (product is null) throw new NotFoundException(nameof(Product), id);
        //var productResponse = _mapper.Map<ProductResponse>(product);

        var product = await _unitOfWork.ProductRepository.FindToIQueryableAsync(c => c.Id == id);
        var productResponse = await _mapper.ProjectTo<ProductResponse>(product).FirstOrDefaultAsync();
        if (productResponse is null) throw new NotFoundException(nameof(Product), id);

        productResponse.IsInWislish = await IsInWishlistAsync(productResponse.Id);

        productResponse.Images.ToList().ForEach(imageResponse =>
             imageResponse.ImageUrl = _generator.GetUriByRouteValues(_httpContextAccessor.HttpContext, Routes.ProductImageRoute, new { imageId = imageResponse.Id })!);

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

        //var userClaimsPrincipal = _httpContextAccessor.HttpContext?.User;
        //if (userClaimsPrincipal is not null)
        //{
        //    var user = await _userManager.GetUserAsync(userClaimsPrincipal);
        //    if (user != null)
        //    {
        //        //paginationProducts.ForEach(product => product.IsInWislish = productIQ.Any(c => c.Wishlists.Where(c => c.ProductId == product.Id && c.UserId == user.Id).Count() > 0));
        //        paginationProducts.ForEach(product => product.IsInWislish = productIQ.Select(c => c.Wishlists.Any(c => c.ProductId == product.Id && c.UserId == user.Id)).FirstOrDefaultAsync().Result);
        //    }
        //}

        paginationProducts.ForEach(product =>
            product.Images.ToList().ForEach(imageResponse =>
                  imageResponse.ImageUrl = _urlHelper.Link(Routes.ProductImageRoute, new { imageId = imageResponse.Id })!));

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

        var imageResponses = _mapper.Map<IList<ImageResponse>>(images);

        imageResponses.ToList().ForEach(imageResponse =>
               imageResponse.ImageUrl = _urlHelper.Link(Routes.ProductImageRoute, new { imageId = imageResponse.Id })!);

        return imageResponses;
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

    public async Task<PaginatedList<ReviewResponse>> GetReviewsAsync(int productId, int pageIndex, int pageSize)
    {
        if (!await _unitOfWork.ProductRepository.ExistsByAsync(c => c.Id == productId))
            throw new NotFoundException(nameof(Product), productId);

        // nếu trong dto có map đến entity khác như trong ReviewResponse thì UserReviewResponse User map từ Review.BookingItem.Booking.User
        // thì nên dùng FindToIQueryableAsync để trả về IQ rùi sau đó dùng ProjectTo để map sang dto
        // vì nếu dùng FindAsync mà trong FindAsync thì nó return tolist() thì nó sẽ viết query trả về entity luôn, và sau đó dùng Map để map entity sang dto mà dto có bóc ra child nào  thì nó viết query cho child đó
        // ví dụ như bên dưới:
        // sau khi FindAsync là nó viết query trả về entity reviews luôn
        //// var reviews = await _unitOfWork.ReviewRepository.FindAsync(c => c.BookingItem.Product.Id == productId);
        // và khi Map như bên dưới thì mà trong ReviewResponse lại có 3 child BookingItem.Booking.User
        // thì nó sẽ viết 3 query để lấy 3 child đó
        // mà reviews là list thì vỡi mỗi review thì nó lại viết 3 query để lấy 3 child đó
        // nếu có 3 review thì nó lại viết 9 query để lấy 9 child đó
        //// return _mapper.Map<IList<ReviewResponse>>(reviews);
        // còn nếu dùng FindToIQueryableAsync thì nó sẽ viết query trả về IQ rùi sau đó dùng ProjectTo để map sang dto, mà trong dto có child nó viết vào query IQ  sau đó nó tổng hợp thành 1 query duy nhất
        var reviews = await _unitOfWork.ReviewRepository.FindToIQueryableAsync(c => c.BookingItem.Product.Id == productId);
        var paginationreviews = await _mapper.ProjectTo<ReviewResponse>(reviews).PaginatedListAsync(pageIndex, pageSize);

        paginationreviews.ForEach(reviewResponse =>
                   reviewResponse.User.AvatarUrl = _urlHelper.Link(Routes.UserAvatarRoute, new { userId = reviewResponse.User.Id }));

        return paginationreviews;
    }
}
