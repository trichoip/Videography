namespace Videography.Application.Interfaces.Services;
public interface IImageService
{
    Task<byte[]?> FindByIdAsync(int imageId);
    Task<byte[]?> FindUserAvatarAsync(int userId);
}
