﻿using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.Carts;
public class CreateCartItemRequest : IMapFrom<CartItem>
{
    public int Quantity { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int ProductId { get; set; }
}
