using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Cosmos;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Returns all page directories as PageDirectoryRoute objects. The results of this 
/// query are cached.
/// </summary>
public class GetAllPageDirectoryRoutesQueryHandler
    : IQueryHandler<GetAllPageDirectoryRoutesQuery, ICollection<PageDirectoryRoute>>
    , IPermissionRestrictedQueryHandler<GetAllPageDirectoryRoutesQuery, ICollection<PageDirectoryRoute>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly PageDirectoryLocaleContext _pageDirectoryLocaleContext;
    private readonly IPageDirectoryRouteMapper _pageDirectoryRouteMapper;

    public GetAllPageDirectoryRoutesQueryHandler(
        CofoundryDbContext dbContext,
        PageDirectoryLocaleContext pageDirectoryLocaleContext, 
        IPageDirectoryRouteMapper pageDirectoryRouteMapper
        )
    {
        _dbContext = dbContext;
        _pageDirectoryLocaleContext = pageDirectoryLocaleContext;
        _pageDirectoryRouteMapper = pageDirectoryRouteMapper;
    }

    public async Task<ICollection<PageDirectoryRoute>> ExecuteAsync(GetAllPageDirectoryRoutesQuery query, IExecutionContext executionContext)
    {
        var dbPageDirectories = await _dbContext
            .PageDirectories
            .AsNoTracking()
            .Include(d => d.AccessRules)
            .ToListAsync();
        foreach( var d in dbPageDirectories ) {
            d.PageDirectoryLocales = await _pageDirectoryLocaleContext.PageDirectoryLocales.Where(x => x.PageDirectoryId == d.PageDirectoryId).ToListAsync();
        }
        var activeWebRoutes = _pageDirectoryRouteMapper.Map(dbPageDirectories);

        return activeWebRoutes;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetAllPageDirectoryRoutesQuery command)
    {
        yield return new PageDirectoryReadPermission();
    }
}
