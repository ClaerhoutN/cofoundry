using Cofoundry.Domain;
using Cofoundry.Domain.Internal;
using Cofoundry.Web.PageBlockTypes.CodeSnippet;
using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web;

public class CodeSnippetDisplayModelMapper : IPageBlockTypeDisplayModelMapper<CodeSnippetDataModel>
{
    private readonly IContentRepository _contentRepository;
    private readonly IEnumerable<ICodeSnippetHtmlEncoder> _codeSnippetHtmlEncoders;
    public CodeSnippetDisplayModelMapper(IContentRepository contentRepository, IEnumerable<ICodeSnippetHtmlEncoder> codeSnippetHtmlEncoders)
    {
        _contentRepository = contentRepository;
        _codeSnippetHtmlEncoders = codeSnippetHtmlEncoders;
    }
    public async Task MapAsync(
        PageBlockTypeDisplayModelMapperContext<CodeSnippetDataModel> context,
        PageBlockTypeDisplayModelMapperResult<CodeSnippetDataModel> result
        )
    {
        var codeLanguages = await _contentRepository.CustomEntities()
                .GetByDefinition<CodeLanguageCustomEntityDefinition>()
                .AsRenderSummaries().ExecuteAsync();
        foreach (var item in context.Items)
        {
            string language = codeLanguages.Single(x => x.CustomEntityId == item.DataModel.CodeLanguageId).Title;
            var codeSnippetHtmlEncoder = _codeSnippetHtmlEncoders.Single(x => x.Language == language);

            var displayModel = new CodeSnippetDisplayModel();
            displayModel.RawHtml = codeSnippetHtmlEncoder.ConvertCodeSnippetToHtml(item.DataModel.Code);
            displayModel.CodeLanguage = language;
            result.Add(item, displayModel);
        }
    }
}
