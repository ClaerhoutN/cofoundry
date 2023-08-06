using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web.PageBlockTypes.CodeSnippet;
public interface ICodeSnippetToken
{
    string Value { get; }
    string Kind { get; }
}
public class CodeSnippetTokenDefault : ICodeSnippetToken
{
    public string Value { get; set; }
    public string Kind { get; set; }
}
