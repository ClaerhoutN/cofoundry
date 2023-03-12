namespace Cofoundry.Domain.Data;

/// <summary>
/// Users can be partitioned into different 'User Areas' that enabled the identity system use by the Cofoundry administration area 
/// to be reused for other purposes, but this isn't a common scenario and often there will only be the Cofoundry UserArea. UserAreas
/// are defined in code by defining an <see cref="IUserAreaDefinition"/>.
/// </summary>
public class UserArea
{
    /// <summary>
    /// 3 character code and primary key.
    /// </summary>
    public string UserAreaCode { get; set; }

    /// <summary>
    /// A human readable name used to describe the user area.
    /// </summary>
    public string Name { get; set; }
}
