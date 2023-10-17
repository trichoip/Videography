using Microsoft.AspNetCore.Mvc;
using Videography.Application.DTOs.Categories;
using Videography.Application.Interfaces.Services;

namespace Videography.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategoriesAsync()
    {
        var categoriesResponse = await _categoryService.GetCategoriesAsync();
        return Ok(categoriesResponse);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponse>> FindById(int id)
    {
        var categoryResponse = await _categoryService.FindByIdAsync(id);
        return Ok(categoryResponse);
    }

    [HttpPut("{id}")]
    //[Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<CategoryResponse>> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest();
        }
        var categoryResponse = await _categoryService.UpdateAsync(request);
        return Ok(categoryResponse);
    }

    [HttpPost]
    //[Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<CategoryResponse>> CreateAsync(CreateCategoryRequest request)
    {
        var categoryResponse = await _categoryService.CreateAsync(request);
        return CreatedAtAction(nameof(FindById), new { id = categoryResponse.Id }, categoryResponse);
    }

    [HttpDelete("{id}")]
    //[Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _categoryService.DeleteAsync(id);
        return NoContent();
    }

}
