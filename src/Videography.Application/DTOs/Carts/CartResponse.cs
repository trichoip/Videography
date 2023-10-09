using Videography.Application.Common.Mappings;
using Videography.Application.DTOs.Users;
using Videography.Domain.Common;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.Carts;
public class CartResponse : BaseEntity, IMapFrom<Cart>
{
    public decimal TotalAmount { get; set; }
    public int TotalQuantity { get; set; }

    public int UserId { get; set; }
    public UserResponse User { get; set; } = default!;
    public ICollection<CartItemDto> CartItems { get; set; } = new HashSet<CartItemDto>();
}
