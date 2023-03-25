using Cofoundry.Core.ResourceFiles;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using System.Collections.Immutable;

namespace Cofoundry.Web;

/// <summary>
/// Extends the MvcRazorRuntimeCompilationOptions configuration adding Cofoundry specific
/// settings such as extending the FileProviders collection using IResourceFileProviderFactory.
/// </summary>
public class CofoundryMvcRazorRuntimeCompilationOptionsConfiguration : IMvcRazorRuntimeCompilationOptionsConfiguration
{
    private readonly IResourceFileProviderFactory _resourceFileProviderFactory;
    private readonly IImmutableList<string> _additionalReferencePaths;

    public CofoundryMvcRazorRuntimeCompilationOptionsConfiguration(
        IResourceFileProviderFactory resourceFileProviderFactory, 
        IEnumerable<IAssemblyResourceRegistration> assemblyResourceRegistrations
        )
    {
        _resourceFileProviderFactory = resourceFileProviderFactory;
        _additionalReferencePaths = EnumerableHelper.Enumerate(assemblyResourceRegistrations).Select(x => x.GetType().Assembly.Location).ToImmutableList();
    }

    public void Configure(MvcRazorRuntimeCompilationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.FileProviders.Add(_resourceFileProviderFactory.Create());
        foreach(string referencePath in _additionalReferencePaths)
            options.AdditionalReferencePaths.Add(referencePath);
    }
}
