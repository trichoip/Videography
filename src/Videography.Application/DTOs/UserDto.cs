using System.ComponentModel.DataAnnotations;
using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;
using Videography.Domain.Enums;

namespace Videography.Application.DTOs;
public class UserDto : IMapFrom<User>
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public byte[] Avatar { get; set; } = default!;

    [EnumDataType(typeof(UserStatus))]
    public UserStatus UserStatus { get; set; }
}
