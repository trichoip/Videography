using AutoMapper;
using Videography.Application.Common.Mappings;
using Videography.Application.DTOs.Users;
using Videography.Domain.Common;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.Reviews;
public class ReviewResponse : BaseEntity, IMapFrom<Review>
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public double Rating { get; set; }
    public UserReviewResponse User { get; set; } = default!;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Review, ReviewResponse>()
               .ForMember(d => d.User, opt => opt.MapFrom(s => s.BookingItem.Booking.User));
    }
}
