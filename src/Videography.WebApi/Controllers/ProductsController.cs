using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Videography.Application.DTOs.Products;
using Videography.Application.Helpers;
using Videography.Application.Interfaces.Services;
using Videography.Application.Specifications;
using Videography.Domain.Constants;
using Videography.WebApi.Attributes;

namespace Videography.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <remarks>
    /// averageRating,desc || totalReviews,asc .....
    /// </remarks>
    [HttpGet]
    public async Task<ActionResult<PaginatedList<ProductResponse>>> GetProductsAsync([FromQuery] ProductSpecPrams productSpecPrams)
    {
        var specification = new ProductWithSpecification(productSpecPrams);

        var paginationProducts = await _productService
            .GetProductsAsync(productSpecPrams.pageIndex,
                              productSpecPrams.pageSize,
                              specification.Criteria,
                              specification.OrderBy);

        //paginationProducts.ForEach(productResponse =>
        //    productResponse.Images.ToList().ForEach(imageResponse =>
        //         imageResponse.ImageUrl = Url.Link(Routes.ProductImageRoute, new { imageId = imageResponse.Id })!));

        //return Ok(new PaginatedResponse<ProductResponse>(paginationProducts));
        return Ok(paginationProducts.ToPaginatedResponse());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> FindById(int id)
    {
        var productResponse = await _productService.FindByIdAsync(id);

        //productResponse?.Images.ToList().ForEach(imageResponse =>
        //    imageResponse.ImageUrl = Url.Link(Routes.ProductImageRoute, new { imageId = imageResponse.Id })!);

        return Ok(productResponse);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<ProductResponse>> UpdateAsync(int id, UpdateProductRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest();
        }
        var productResponse = await _productService.UpdateAsync(request);
        return Ok(productResponse);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<ProductResponse>> CreateAsync(CreateProductRequest request)
    {
        var productResponse = await _productService.CreateAsync(request);
        return CreatedAtAction(nameof(FindById), new { id = productResponse.Id }, productResponse);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/Images")]
    public async Task<IActionResult> GetImagesAsync(int id)
    {
        var imageResponses = (await _productService.GetImagesAsync(id)).ToList();

        //imageResponses.ForEach(imageResponse =>
        //        imageResponse.ImageUrl = Url.Link(Routes.ProductImageRoute, new { imageId = imageResponse.Id })!);

        return Ok(imageResponses);
    }

    [HttpDelete("{id}/Images/{imageId}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> RemoveImageAsync(int id, int imageId)
    {
        await _productService.RemoveImageAsync(id, imageId);
        return Ok(new { StatusMessage = $"Product {id} Remove image {imageId} succesfully." });
    }

    [HttpDelete("{id}/Images")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> RemoveImagesAsync(int id)
    {
        await _productService.RemoveImagesAsync(id);
        return Ok(new { StatusMessage = $"Product {id} Remove all images succesfully." });
    }

    [HttpPost("{id}/Image")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> AddImageAsync(
        int id, [FileValidation(2 * 1024 * 1024, ".png", ".jpg", ".jpeg", ".gif")] IFormFile image)
    {
        await _productService.AddImageAsync(id, image);
        return Ok(new { StatusMessage = "Upload image successfuly" });
    }

    [HttpPost("{id}/Images")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> AddImagesAsync(
        int id, [FileValidation(2 * 1024 * 1024, ".png", ".jpg", ".jpeg", ".gif")] IFormFileCollection images)
    {
        await _productService.AddImagesAsync(id, images);
        return Ok(new { StatusMessage = "Upload images successfuly" });
    }

    [HttpGet("{id}/Reviews")]
    public async Task<IActionResult> GetReviewsAsync(int id, int pageIndex, int pageSize)
    {
        var paginationReviews = await _productService.GetReviewsAsync(id, pageIndex, pageSize);
        return Ok(paginationReviews.ToPaginatedResponse());
    }

    [HttpGet("{id}/Bookings")]
    public async Task<IActionResult> GetValidBookingItemsAsync(int id)
    {
        var bookingItemValidResponses = await _productService.GetValidBookingItemsAsync(id);
        return Ok(bookingItemValidResponses);
    }

}
