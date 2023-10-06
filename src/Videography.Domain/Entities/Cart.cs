using Videography.Domain.Common;

namespace Videography.Domain.Entities;
public class Cart : BaseEntity
{
    public decimal TotalAmount { get; set; }
    public int TotalQuantity { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;
    public virtual ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();
}
