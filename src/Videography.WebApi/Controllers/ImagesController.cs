﻿using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Videography.Application.Interfaces.Services;

namespace Videography.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ImagesController : ControllerBase
{
    private readonly IImageService _imageService;
    public ImagesController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpGet("{imageId}", Name = nameof(GetImageAsync))]
    public async Task<IActionResult> GetImageAsync(int imageId)
    {
        var image = await _imageService.FindByIdAsync(imageId);
        return File(image, MediaTypeNames.Image.Jpeg);
    }

    [HttpGet("User/{userId}", Name = nameof(GetUserAvatarAsync))]
    public async Task<IActionResult> GetUserAvatarAsync(int userId)
    {
        var avatar = await _imageService.FindUserAvatarAsync(userId);
        return File(avatar, MediaTypeNames.Image.Jpeg);
    }
}
