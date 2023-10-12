using Microsoft.EntityFrameworkCore;
using Videography.Application.Helpers;

namespace Videography.Application.Common.Mappings;
public static class MappingExtensions
{
    // có đổi name pageIndex , nhớ đổi lại trong project Aspnet core
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable, int pageIndex, int pageSize) where TDestination : class
      => PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), pageIndex, pageSize);
}
