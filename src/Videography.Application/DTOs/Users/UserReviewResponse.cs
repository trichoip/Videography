using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.Users;
public class UserReviewResponse : IMapFrom<User>
{
    public int Id { get; set; }
    public string UserName { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string? AvatarUrl { get; set; }
}
