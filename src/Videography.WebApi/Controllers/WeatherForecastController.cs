using Microsoft.AspNetCore.Mvc;
using Videography.Application.DTOs;

namespace Videography.WebApi.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpPost]
    public IActionResult actionResult(CategoryDto categoryDto)
    {

        return Ok(categoryDto);
    }
}
