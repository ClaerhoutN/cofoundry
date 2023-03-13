using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Cosmos;
using Cofoundry.Domain.Internal;
using System.IO;
using System.Runtime.CompilerServices;

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
    private readonly PageDirectoryAccessRuleContext _pageDirectoryAccessRuleContext;
    private readonly PageDirectoryPathContext _pageDirectoryPathContext;
    private readonly IEntityAccessRuleSetDetailsMapper _entityAccessDetailsMapper;
    private readonly IPageDirectoryMicroSummaryMapper _pageDirectoryMicroSummaryMapper;

    public GetPageDirectoryAccessDetailsByPageDirectoryIdQueryHandler(
        CofoundryDbContext dbContext,
        PageDirectoryPathContext pageDirectoryPathContext,
        PageDirectoryAccessRuleContext pageDirectoryAccessRuleContext, 
        IEntityAccessRuleSetDetailsMapper entityAccessDetailsMapper,
        IPageDirectoryMicroSummaryMapper pageDirectoryMicroSummaryMapper
        )
    {
        _dbContext = dbContext;
        _pageDirectoryAccessRuleContext = pageDirectoryAccessRuleContext;
        _pageDirectoryPathContext = pageDirectoryPathContext;
        _entityAccessDetailsMapper = entityAccessDetailsMapper;
        _pageDirectoryMicroSummaryMapper = pageDirectoryMicroSummaryMapper;
    }

    public async Task<PageDirectoryAccessRuleSetDetails> ExecuteAsync(GetPageDirectoryAccessDetailsByPageDirectoryIdQuery query, IExecutionContext executionContext)
    {
        var dbDirectory = await _dbContext
            .PageDirectories
            .AsNoTracking()
            .FilterById(query.PageDirectoryId)
            .SingleOrDefaultAsync();

        if (dbDirectory == null) return null;

        await SetPageDirectoryAccessRules(dbDirectory);

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
            .FilterByDescendantId(dbPageDirectory.PageDirectoryId)
            .FilterNotSelfReferencing()
            .ToListAsync();

        result.InheritedAccessRules = new List<InheritedPageDirectoryAccessDetails>();

        foreach (var dbInheritedRule in dbInheritedRules)
        {
            if (dbInheritedRule.AncestorPageDirectory != null) 
            {
                dbInheritedRule.AncestorPageDirectory.PageDirectoryPath = await _pageDirectoryPathContext.PageDirectoryPaths.AsNoTracking().FirstOrDefaultAsync(x => x.PageDirectoryId == dbInheritedRule.AncestorPageDirectory.PageDirectoryId);
                await SetPageDirectoryAccessRules(dbInheritedRule.AncestorPageDirectory);
            }
            if (dbInheritedRule.DescendantPageDirectoryId != dbPageDirectory.PageDirectoryId || !dbInheritedRule.AncestorPageDirectory.AccessRules.Any())
                continue;
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

    private Task SetPageDirectoryAccessRules(PageDirectory pageDirectory) => SetPageDirectoryAccessRules(new[] { pageDirectory });
    private async Task SetPageDirectoryAccessRules(IEnumerable<PageDirectory> pageDirectories) 
    {
        foreach(var pageDirectory in pageDirectories) 
        {
            pageDirectory.AccessRules = await _pageDirectoryAccessRuleContext.PageDirectoryAccessRules.Where(x => x.PageDirectoryId == pageDirectory.PageDirectoryId).ToListAsync();
        }
    }
}
