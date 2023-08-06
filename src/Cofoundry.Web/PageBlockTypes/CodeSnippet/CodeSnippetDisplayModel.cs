using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web;

public class CodeSnippetDisplayModel : IPageBlockTypeDisplayModel
{
    public IHtmlContent RawHtml { get; set; }
    public string CodeLanguage { get; set; }
}
