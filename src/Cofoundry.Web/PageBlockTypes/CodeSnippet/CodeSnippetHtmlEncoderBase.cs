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
            sb.Append(WrapWithTags(
                HtmlFormatter.ConvertLineBreaksToBrTags(
                    HttpUtility.HtmlEncode(token.Value)), token.Kind));
        }

        return new HtmlString(sb.ToString());
    }
    protected abstract string WrapWithTags(string s, string tokenKind);
    protected abstract IEnumerable<ICodeSnippetToken> GetTokens(string code);
    public abstract string Language { get; }
}
