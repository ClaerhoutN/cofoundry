using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web;
public class CodeLanguageCustomEntityDefinition : ICustomEntityDefinition<CodeLanguageDataModel>
{
    public const string DefinitionCode = "SIMCLN";
    public string CustomEntityDefinitionCode => DefinitionCode;
    public string Name => "Code language";
    public string NamePlural => "Code languages";
    public string Description => "Used to indicate the language of a code snippet.";
    public bool ForceUrlSlugUniqueness => true;
    public bool HasLocale => false;
    public bool AutoGenerateUrlSlug => true;
    public bool AutoPublish => true;
}
