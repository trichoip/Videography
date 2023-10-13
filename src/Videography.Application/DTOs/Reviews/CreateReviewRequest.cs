using AutoMapper;
using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.Reviews;
public class CreateReviewRequest : IMapFrom<Review>
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public double Rating { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CreateReviewRequest, Review>()
               .ForMember(x => x.Rating, c => c.MapFrom(x => Math.Round(x.Rating, 1)));
    }
}
