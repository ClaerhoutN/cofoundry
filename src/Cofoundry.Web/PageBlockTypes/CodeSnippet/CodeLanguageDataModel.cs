using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web;
public class CodeLanguageDataModel : ICustomEntityDataModel
{
    [Display(Description = "Display name")]
    public string DisplayName { get; set; }
    [Display(Description = "highlight.js alias")]
    public string HighlightJSAlias { get; set; }
}
