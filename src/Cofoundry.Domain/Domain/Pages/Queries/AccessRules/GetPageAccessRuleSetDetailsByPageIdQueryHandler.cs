using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Cosmos;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Query that returns detailed information about access restrictions
/// configured for a page, including all access rules as well as those 
/// inherited from parent directories.
/// </summary>
public class GetPageAccessRuleSetDetailsByPageIdQueryHandler
    : IQueryHandler<GetPageAccessRuleSetDetailsByPageIdQuery, PageAccessRuleSetDetails>
    , IPermissionRestrictedQueryHandler<GetPageAccessRuleSetDetailsByPageIdQuery, PageAccessRuleSetDetails>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly PageDirectoryPathContext _pageDirectoryPathContext;
    private readonly IEntityAccessRuleSetDetailsMapper _entityAccessDetailsMapper;
    private readonly IPageDirectoryMicroSummaryMapper _pageDirectoryMicroSummaryMapper;

    public GetPageAccessRuleSetDetailsByPageIdQueryHandler(
        CofoundryDbContext dbContext,
        PageDirectoryPathContext pageDirectoryPathContext, 
        IEntityAccessRuleSetDetailsMapper entityAccessDetailsMapper,
        IPageDirectoryMicroSummaryMapper pageDirectoryMicroSummaryMapper
        )
    {
        _dbContext = dbContext;
        _pageDirectoryPathContext = pageDirectoryPathContext;
        _entityAccessDetailsMapper = entityAccessDetailsMapper;
        _pageDirectoryMicroSummaryMapper = pageDirectoryMicroSummaryMapper;
    }

    public async Task<PageAccessRuleSetDetails> ExecuteAsync(GetPageAccessRuleSetDetailsByPageIdQuery query, IExecutionContext executionContext)
    {
        var dbPage = await _dbContext
            .Pages
            .AsNoTracking()
            .Include(p => p.AccessRules)
            .FilterActive()
            .FilterById(query.PageId)
            .SingleOrDefaultAsync();

        if (dbPage == null) return null;

        var result = new PageAccessRuleSetDetails();
        await _entityAccessDetailsMapper.MapAsync(dbPage, result, executionContext, (dbRule, rule) =>
        {
            rule.PageId = dbRule.PageId;
            rule.PageAccessRuleId = dbRule.PageAccessRuleId;
        });

        result.PageId = dbPage.PageId;
        await MapInheritedRules(dbPage, result, executionContext);

        return result;
    }

    private async Task MapInheritedRules(Page dbPage, PageAccessRuleSetDetails result, IExecutionContext executionContext)
    {
        var dbInheritedRules = await _dbContext
            .PageDirectoryClosures
            .AsNoTracking()
            .Include(d => d.AncestorPageDirectory)
            .ThenInclude(d => d.AccessRules)
            .Include(d => d.AncestorPageDirectory)
            .FilterByDescendantId(dbPage.PageDirectoryId)
            .Where(d => d.DescendantPageDirectoryId == dbPage.PageDirectoryId && d.AncestorPageDirectory.AccessRules.Any())
            .OrderByDescending(d => d.Distance)
            .ToListAsync();

        result.InheritedAccessRules = new List<InheritedPageDirectoryAccessDetails>();

        foreach (var dbInheritedRule in dbInheritedRules)
        {
            if (dbInheritedRule.AncestorPageDirectory != null)
                dbInheritedRule.AncestorPageDirectory.PageDirectoryPath = await _pageDirectoryPathContext.PageDirectoryPaths.AsNoTracking().FirstOrDefaultAsync(x => x.PageDirectoryId == dbInheritedRule.AncestorPageDirectory.PageDirectoryId);
            var inheritedDirectory = new InheritedPageDirectoryAccessDetails();
            inheritedDirectory.PageDirectory = _pageDirectoryMicroSummaryMapper.Map(dbInheritedRule.AncestorPageDirectory);
            await _entityAccessDetailsMapper.MapAsync(dbInheritedRule.AncestorPageDirectory, inheritedDirectory, executionContext, (dbRule, rule) =>
            {
                rule.PageDirectoryId = dbRule.PageDirectoryId;
                rule.PageDirectoryAccessRuleId = dbRule.PageDirectoryAccessRuleId;
            });

            result.InheritedAccessRules.Add(inheritedDirectory);
        }
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageAccessRuleSetDetailsByPageIdQuery query)
    {
        yield return new PageReadPermission();
    }
}
