using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web.PageBlockTypes.CodeSnippet;
public interface ICodeSnippetToken
{
    string Value { get; }
    string CssClass { get; }
}
public class CodeSnippetTokenDefault : ICodeSnippetToken
{
    public string Value { get; set; }
    public string CssClass { get; } = "defaultToken";
}
