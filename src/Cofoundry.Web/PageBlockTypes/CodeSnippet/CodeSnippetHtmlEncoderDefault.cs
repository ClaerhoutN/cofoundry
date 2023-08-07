using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web.PageBlockTypes.CodeSnippet;
public class CodeSnippetHtmlEncoderDefault : CodeSnippetHtmlEncoderBase
{
    public override bool RequiresHighlighting => true;
    public override string Language => "default";

    protected override IEnumerable<ICodeSnippetToken> GetTokens(string code)
    {
        return new ICodeSnippetToken[] { new CodeSnippetTokenDefault { Value = code } };
    }
}
