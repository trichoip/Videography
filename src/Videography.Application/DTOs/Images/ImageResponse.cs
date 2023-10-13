using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.Images;
public class ImageResponse : IMapFrom<Image>
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = default!;

}
