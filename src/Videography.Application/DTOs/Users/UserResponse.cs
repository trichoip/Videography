using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;
using Videography.Domain.Enums;

namespace Videography.Application.DTOs.Users;
public class UserResponse : IMapFrom<User>
{
    public int Id { get; set; }
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool EmailConfirmed { get; set; }
    public string PhoneNumber { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public UserStatus Status { get; set; }
    public string? AvatarUrl { get; set; }
    public int TotalQuantityItemInCart { get; set; }
}