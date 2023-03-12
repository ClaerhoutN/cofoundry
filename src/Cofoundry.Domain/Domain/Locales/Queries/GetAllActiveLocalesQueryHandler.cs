using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Cosmos;

namespace Cofoundry.Domain.Internal;

public class GetAllActiveLocalesQueryHandler
    : IQueryHandler<GetAllActiveLocalesQuery, ICollection<ActiveLocale>>
    , IIgnorePermissionCheckHandler
{
    private readonly LocaleContext _localeContext;
    private readonly ILocaleCache _cache;
    private readonly IActiveLocaleMapper _activeLocaleMapper;

    public GetAllActiveLocalesQueryHandler(
        LocaleContext localeContext,
        ILocaleCache cache,
        IActiveLocaleMapper activeLocaleMapper
        )
    {
        _localeContext = localeContext;
        _cache = cache;
        _activeLocaleMapper = activeLocaleMapper;
    }

    public async Task<ICollection<ActiveLocale>> ExecuteAsync(GetAllActiveLocalesQuery query, IExecutionContext executionContext)
    {
        var results = await _cache.GetOrAddAsync(new Func<Task<ICollection<ActiveLocale>>>(async () =>
        {
            var dbResults = await GetAllLocales().ToListAsync();
            var mappedResults = dbResults
                .Select(_activeLocaleMapper.Map)
                .ToList();

            return mappedResults;
        }));

        return results;
    }

    private IQueryable<Locale> GetAllLocales()
    {
        return _localeContext
                .Locales
                .AsNoTracking()
                .Where(l => l.IsActive)
                .OrderBy(l => l.LocaleName);
    }
}
