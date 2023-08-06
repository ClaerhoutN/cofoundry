using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cofoundry.Web.PageBlockTypes.CodeSnippet;
public interface ICodeSnippetHtmlEncoder
{
    /// <summary>
    /// Converts a code snippet to html with a rich color scheme for the specified coding language
    /// </summary>
    /// <param name="s">code snippet to convert</param>
    /// <returns>HtmlString version of the input string formatted to html.</returns>
    IHtmlContent ConvertCodeSnippetToHtml(string s);
    string Language { get; }
}
