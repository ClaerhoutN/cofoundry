using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Cosmos;

namespace Cofoundry.Domain.Internal;

public class EnsureUserAreaExistsCommandHandler
    : ICommandHandler<EnsureUserAreaExistsCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly UserAreaContext _userAreaContext;
    private readonly IUserAreaDefinitionRepository _userAreaRepository;

    public EnsureUserAreaExistsCommandHandler(
        UserAreaContext userAreaContext,
        IUserAreaDefinitionRepository userAreaRepository
        )
    {
        _userAreaContext = userAreaContext;
        _userAreaRepository = userAreaRepository;
    }

    public async Task ExecuteAsync(EnsureUserAreaExistsCommand command, IExecutionContext executionContext)
    {
        var userArea = _userAreaRepository.GetRequiredByCode(command.UserAreaCode);
        EntityNotFoundException.ThrowIfNull(userArea, command.UserAreaCode);

        var dbUserArea = await _userAreaContext
            .UserAreas
            .SingleOrDefaultAsync(a => a.UserAreaCode == userArea.UserAreaCode);

        if (dbUserArea == null)
        {
            dbUserArea = new UserArea();
            dbUserArea.UserAreaCode = userArea.UserAreaCode;
            dbUserArea.Name = userArea.Name;

            _userAreaContext.UserAreas.Add(dbUserArea);
        }
    }
}
