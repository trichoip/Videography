using Videography.Application.Common.Mappings;
using Videography.Application.DTOs.Products;
using Videography.Application.DTOs.Users;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs;
public class WishlistDto : IMapFrom<Wishlist>
{
    public int UserId { get; set; }
    public int ProductId { get; set; }

    public virtual UserResponse User { get; set; } = default!;
    public virtual ProductResponse Product { get; set; } = default!;
}
