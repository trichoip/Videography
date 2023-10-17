using Microsoft.AspNetCore.Mvc;
using Videography.Application.DTOs.CreditCardTypes;
using Videography.Application.Interfaces.Services;

namespace Videography.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class CreditCardTypesController : ControllerBase
{
    private readonly ICreditCardTypeService _creditCardTypeService;
    public CreditCardTypesController(ICreditCardTypeService creditCardTypeService)
    {
        _creditCardTypeService = creditCardTypeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CreditCardTypeResponse>>> GetCreditCardTypesAsync()
    {
        var categoriesResponse = await _creditCardTypeService.GetCreditCardTypesAsync();
        return Ok(categoriesResponse);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CreditCardTypeResponse>> FindById(int id)
    {
        var creditCardTypeResponse = await _creditCardTypeService.FindByIdAsync(id);
        return Ok(creditCardTypeResponse);
    }

    [HttpPut("{id}")]
    //[Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<CreditCardTypeResponse>> UpdateAsync(int id, UpdateCreditCardTypeRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest();
        }
        var creditCardTypeResponse = await _creditCardTypeService.UpdateAsync(request);
        return Ok(creditCardTypeResponse);
    }

    [HttpPost]
    //[Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<CreditCardTypeResponse>> CreateAsync(CreateCreditCardTypeRequest request)
    {
        var creditCardTypeResponse = await _creditCardTypeService.CreateAsync(request);
        return CreatedAtAction(nameof(FindById), new { id = creditCardTypeResponse.Id }, creditCardTypeResponse);
    }

    [HttpDelete("{id}")]
    //[Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _creditCardTypeService.DeleteAsync(id);
        return NoContent();
    }
}
