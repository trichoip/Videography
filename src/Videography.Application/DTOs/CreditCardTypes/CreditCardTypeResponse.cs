﻿using Videography.Application.Common.Mappings;
using Videography.Domain.Entities;

namespace Videography.Application.DTOs.CreditCardTypes;
public class CreditCardTypeResponse : IMapFrom<CreditCardType>
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}
