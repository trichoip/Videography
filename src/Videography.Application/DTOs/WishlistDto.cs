using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs;
public class WishlistDto : IMapFrom<Wishlist>
{
    public int UserId { get; set; }
    public int ProductId { get; set; }

    public virtual UserDto User { get; set; } = default!;
    public virtual ProductDto Product { get; set; } = default!;
}
