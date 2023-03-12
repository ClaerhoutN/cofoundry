namespace Cofoundry.Domain.Data;

public class PageDirectoryLocale
{
    public int PageDirectoryLocaleId { get; set; }
    public int PageDirectoryId { get; set; }
    public int LocaleId { get; set; }
    public string UrlPath { get; set; }

    public System.DateTime CreateDate { get; set; }
    public int CreatorId { get; set; }
}
