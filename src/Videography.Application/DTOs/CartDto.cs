﻿using Videography.Domain.Common;

namespace Videography.Application.DTOs;
public class CartDto : BaseEntity
{
    public decimal TotalAmount { get; set; }
    public int TotalQuantity { get; set; }

    public int UserId { get; set; }
    public virtual UserDto User { get; set; } = default!;
    public virtual ICollection<CartItemDto> CartItems { get; set; } = new HashSet<CartItemDto>();
}