using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web;

public class CodeSnippetDisplayModel : IPageBlockTypeDisplayModel
{
    public IHtmlContent RawHtml { get; set; }
    public string CodeLanguage { get; set; }
    public string HightlightJSAlias { get; set; }
    public bool RequiresHighlighting { get; set; }
}
