using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.Wishlists;
public class CreateWishlistItemRequest : IMapFrom<Wishlist>
{
    public int ProductId { get; set; }
}
