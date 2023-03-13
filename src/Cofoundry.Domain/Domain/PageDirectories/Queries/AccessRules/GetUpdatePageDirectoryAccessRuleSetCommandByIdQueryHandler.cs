using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Cosmos;
using System.IO;

namespace Cofoundry.Domain.Internal;

public class GetUpdatePageDirectoryAccessRuleSetCommandByIdQueryHandler
    : IQueryHandler<GetPatchableCommandByIdQuery<UpdatePageDirectoryAccessRuleSetCommand>, UpdatePageDirectoryAccessRuleSetCommand>
    , IPermissionRestrictedQueryHandler<GetPatchableCommandByIdQuery<UpdatePageDirectoryAccessRuleSetCommand>, UpdatePageDirectoryAccessRuleSetCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly PageDirectoryAccessRuleContext _pageDirectoryAccessRuleContext;
    public GetUpdatePageDirectoryAccessRuleSetCommandByIdQueryHandler(
        CofoundryDbContext dbContext,
        PageDirectoryAccessRuleContext pageDirectoryAccessRuleContext
        )
    {
        _dbContext = dbContext;
        _pageDirectoryAccessRuleContext = pageDirectoryAccessRuleContext;
    }

    public async Task<UpdatePageDirectoryAccessRuleSetCommand> ExecuteAsync(GetPatchableCommandByIdQuery<UpdatePageDirectoryAccessRuleSetCommand> query, IExecutionContext executionContext)
    {
        var dbPageDirectory = await _dbContext
            .PageDirectories
            .AsNoTracking()
            .FilterById(query.Id)
            .SingleOrDefaultAsync();

        if (dbPageDirectory == null) return null;

        await SetPageDirectoryAccessRules(dbPageDirectory);

        var violationAction = EnumParser.ParseOrNull<AccessRuleViolationAction>(dbPageDirectory.AccessRuleViolationActionId);
        if (!violationAction.HasValue)
        {
            throw new InvalidOperationException($"{nameof(AccessRuleViolationAction)} of value {dbPageDirectory.AccessRuleViolationActionId} could not be parsed on a page directory with an id of {dbPageDirectory.PageDirectoryId}.");
        }

        var command = new UpdatePageDirectoryAccessRuleSetCommand()
        {
            PageDirectoryId = dbPageDirectory.PageDirectoryId,
            UserAreaCodeForSignInRedirect = dbPageDirectory.UserAreaCodeForSignInRedirect,
            ViolationAction = violationAction.Value
        };

        command.AccessRules = dbPageDirectory
            .AccessRules
            .Select(r => new UpdatePageDirectoryAccessRuleSetCommand.AddOrUpdatePageDirectoryAccessRuleCommand()
            {
                PageDirectoryAccessRuleId = r.PageDirectoryAccessRuleId,
                UserAreaCode = r.UserAreaCode,
                RoleId = r.RoleId
            })
            .ToList();

        return command;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPatchableCommandByIdQuery<UpdatePageDirectoryAccessRuleSetCommand> query)
    {
        yield return new PageDirectoryReadPermission();
    }

    private Task SetPageDirectoryAccessRules(PageDirectory pageDirectory) => SetPageDirectoryAccessRules(new[] { pageDirectory });
    private async Task SetPageDirectoryAccessRules(IEnumerable<PageDirectory> pageDirectories) {
        foreach (var pageDirectory in pageDirectories) {
            pageDirectory.AccessRules = await _pageDirectoryAccessRuleContext.PageDirectoryAccessRules.Where(x => x.PageDirectoryId == pageDirectory.PageDirectoryId).ToListAsync();
        }
    }
}
