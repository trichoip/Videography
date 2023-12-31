﻿using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Videography.Application.Specifications;
public interface ISpecifications<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    List<Expression<Func<IQueryable<T>, IIncludableQueryable<T, object>>>> Includes { get; }
    Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; }
}
