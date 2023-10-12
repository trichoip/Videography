using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.Images;
public class ImageResponse : IMapFrom<Image>
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = default!;

    //public void Mapping(Profile profile)
    //{
    //    profile.CreateMap<Image, ImageResponse>()
    //        .ForMember(d => d.ImageUrl, opt => opt.MapFrom(s => s.Id))
    //        .ReverseMap();
    //}

}
