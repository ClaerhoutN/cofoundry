using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Cosmos;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Returns all access rules associated with a specific page using a default
/// ordering of specificity i.e. with user area rules before role-based rules.
/// </summary>
public class GetPageDirectoryAccessDetailsByPageDirectoryIdQueryHandler
    : IQueryHandler<GetPageDirectoryAccessDetailsByPageDirectoryIdQuery, PageDirectoryAccessRuleSetDetails>
    , IPermissionRestrictedQueryHandler<GetPageDirectoryAccessDetailsByPageDirectoryIdQuery, PageDirectoryAccessRuleSetDetails>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly PageDirectoryPathContext _pageDirectoryPathContext;
    private readonly IEntityAccessRuleSetDetailsMapper _entityAccessDetailsMapper;
    private readonly IPageDirectoryMicroSummaryMapper _pageDirectoryMicroSummaryMapper;

    public GetPageDirectoryAccessDetailsByPageDirectoryIdQueryHandler(
        CofoundryDbContext dbContext,
        PageDirectoryPathContext pageDirectoryPathContext, 
        IEntityAccessRuleSetDetailsMapper entityAccessDetailsMapper,
        IPageDirectoryMicroSummaryMapper pageDirectoryMicroSummaryMapper
        )
    {
        _dbContext = dbContext;
        pageDirectoryPathContext = _pageDirectoryPathContext;
        _entityAccessDetailsMapper = entityAccessDetailsMapper;
        _pageDirectoryMicroSummaryMapper = pageDirectoryMicroSummaryMapper;
    }

    public async Task<PageDirectoryAccessRuleSetDetails> ExecuteAsync(GetPageDirectoryAccessDetailsByPageDirectoryIdQuery query, IExecutionContext executionContext)
    {
        var dbDirectory = await _dbContext
            .PageDirectories
            .AsNoTracking()
            .Include(d => d.AccessRules)
            .FilterById(query.PageDirectoryId)
            .SingleOrDefaultAsync();

        if (dbDirectory == null) return null;

        var result = new PageDirectoryAccessRuleSetDetails();
        await _entityAccessDetailsMapper.MapAsync(dbDirectory, result, executionContext, (dbRule, rule) =>
        {
            rule.PageDirectoryId = dbRule.PageDirectoryId;
            rule.PageDirectoryAccessRuleId = dbRule.PageDirectoryAccessRuleId;
        });

        result.PageDirectoryId = dbDirectory.PageDirectoryId;
        await MapInheritedRules(dbDirectory, result, executionContext);

        return result;
    }

    private async Task MapInheritedRules(PageDirectory dbPageDirectory, PageDirectoryAccessRuleSetDetails result, IExecutionContext executionContext)
    {
        var dbInheritedRules = await _dbContext
            .PageDirectoryClosures
            .AsNoTracking()
            .Include(d => d.AncestorPageDirectory)
            .ThenInclude(d => d.AccessRules)
            .Include(d => d.AncestorPageDirectory)
            .FilterByDescendantId(dbPageDirectory.PageDirectoryId)
            .FilterNotSelfReferencing()
            .Where(d => d.DescendantPageDirectoryId == dbPageDirectory.PageDirectoryId && d.AncestorPageDirectory.AccessRules.Any())
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

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageDirectoryAccessDetailsByPageDirectoryIdQuery query)
    {
        yield return new PageDirectoryReadPermission();
    }
}
