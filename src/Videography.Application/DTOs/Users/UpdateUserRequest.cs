using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.Users;
public class UpdateUserRequest : IMapFrom<User>
{
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string FullName { get; set; } = default!;
}
