using Videography.Domain.Common;

namespace Videography.Domain.Entities;
public class CartItem : BaseEntity
{
    public int Quantity { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public int UserId { get; set; }
    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = default!;
    public virtual User User { get; set; } = default!;
}
