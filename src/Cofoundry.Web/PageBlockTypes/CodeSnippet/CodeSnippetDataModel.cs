namespace Cofoundry.Web;

/// <summary>
/// Data model representing text entry with simple formatting like headings and lists
/// </summary>
public class CodeSnippetDataModel : IPageBlockTypeDataModel
{
    [Required, Display(Name = "Code")]
    [MultiLineText]
    public string Code { get; set; }

    [Display(Name = "Code language"/*, Description = "Test Single Category."*/)]
    [CustomEntity(CodeLanguageCustomEntityDefinition.DefinitionCode)]
    public int CodeLanguageId { get; set; }

}
