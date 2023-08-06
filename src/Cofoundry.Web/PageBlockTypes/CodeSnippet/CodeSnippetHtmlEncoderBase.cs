using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cofoundry.Web.PageBlockTypes.CodeSnippet;
public abstract class CodeSnippetHtmlEncoderBase : ICodeSnippetHtmlEncoder
{
    public IHtmlContent ConvertCodeSnippetToHtml(string s)
    {
        if (string.IsNullOrEmpty(s)) return new HtmlString(string.Empty);
        StringBuilder sb = new StringBuilder();
        foreach(var token in GetTokens(s))
        {
            string encoded = HttpUtility.HtmlEncode(token.Value);
            encoded = HtmlFormatter.ConvertLineBreaksToBrTags(encoded);
            encoded = HtmlFormatter.ConvertSpacesToNbsp(encoded);
            sb.Append(WrapWithTags(encoded, token.CssClass));
        }

        return new HtmlString(sb.ToString());
    }
    private string WrapWithTags(string s, string cssClass) => $"<span class=\"{cssClass}\">{s}</span>";
    protected abstract IEnumerable<ICodeSnippetToken> GetTokens(string code);
    public abstract string Language { get; }
}
