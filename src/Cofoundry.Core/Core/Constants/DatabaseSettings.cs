using Cofoundry.Core.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Core;

/// <summary>
/// Settings to use when connecting to the Cofoundry database.
/// </summary>
public class DatabaseSettings : CofoundryConfigurationSettingsBase
{
    /// <summary>
    /// The connection string to the Cofoundry database.
    /// </summary>
    [Required]
    public string ConnectionStringSQLServer { get; set; }
    [Required]
    public string ConnectionStringCosmosDb { get; set; }
    [Required]
    public string DatabaseNameCosmosDb { get; set; }
}
