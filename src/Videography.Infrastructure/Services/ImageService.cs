using Videography.Application.Common.Exceptions;
using Videography.Application.Interfaces.Repositories;
using Videography.Application.Interfaces.Services;
using Videography.Domain.Entities;

namespace Videography.Infrastructure.Services;
public class ImageService : IImageService
{
    private readonly IUnitOfWork _unitOfWork;
    public ImageService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<byte[]> FindByIdAsync(int imageId)
    {
        var image = await _unitOfWork.ImageRepository.FindByIdAsync(imageId);
        if (image == null)
        {
            throw new NotFoundException(nameof(Image), imageId);
        }
        if (image.Data == null)
        {
            throw new NotFoundException($"image {image.Id} error.");
        }
        return image.Data;
    }

    public async Task<byte[]> FindUserAvatarAsync(int userId)
    {
        var user = await _unitOfWork.UserRepository.FindByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException(nameof(User), userId);
        }

        if (user.Avatar == null)
        {
            throw new NotFoundException($"user {user.Id} avatar error.");
        }
        return user.Avatar;
    }

}
