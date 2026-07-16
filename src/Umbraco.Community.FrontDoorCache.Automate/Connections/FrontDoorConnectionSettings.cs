using Umbraco.Automate.Core.Settings;

namespace Umbraco.Community.FrontDoorCache.Automate.Connections;

/// <summary>
/// Settings for the Azure Front Door connection type.
/// </summary>
public sealed class FrontDoorConnectionSettings
{
    /// <summary>
    /// Gets or sets the ID of the Azure subscription that the Front Door profile belongs to.
    /// </summary>
    [Field(Label = "Subscription ID", Description = "The ID of the Azure subscription that the Front Door profile belongs to.", SortOrder = 10)]
    public string SubscriptionId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the Azure resource group that the Front Door profile belongs to.
    /// </summary>
    [Field(Label = "Resource Group Name", Description = "The name of the Azure resource group that the Front Door profile belongs to.", SortOrder = 20)]
    public string ResourceGroupName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the Front Door profile to purge.
    /// </summary>
    [Field(Label = "Front Door Profile Name", Description = "The name of the Front Door profile to purge.", SortOrder = 30)]
    public string FrontDoorName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the endpoint within the Front Door profile to purge.
    /// </summary>
    [Field(Label = "Endpoint Name", Description = "The name of the endpoint within the Front Door profile to purge.", SortOrder = 40)]
    public string EndpointName { get; set; } = string.Empty;
}
