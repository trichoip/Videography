using Videography.Domain.Common;

namespace Videography.Application.DTOs;
public class CartItemDto : BaseEntity
{
    public int Quantity { get; set; }
    public decimal Amount { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int CartId { get; set; }
    public int ProductId { get; set; }

    public virtual ProductDto Product { get; set; } = default!;
    public virtual CartDto Cart { get; set; } = default!;
}
