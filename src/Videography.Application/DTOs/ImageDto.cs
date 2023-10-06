using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs;
public class ImageDto : IMapFrom<Image>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public byte[] Data { get; set; } = default!;

}
