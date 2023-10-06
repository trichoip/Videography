using Videography.Domain.Common;

namespace Videography.Domain.Entities;
public class CartItem : BaseEntity
{
    public int Quantity { get; set; }
    public decimal Amount { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int CartId { get; set; }
    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = default!;
    public virtual Cart Cart { get; set; } = default!;
}
