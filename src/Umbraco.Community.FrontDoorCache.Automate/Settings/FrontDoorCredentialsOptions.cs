namespace Umbraco.Community.FrontDoorCache.Automate.Settings;

/// <summary>
/// Microsoft Entra ID app registration credentials used to authenticate against the Azure
/// Front Door management API. Bound from the <c>FrontDoor:Cache:Credentials</c> configuration
/// section rather than stored on the Connection, so secrets stay out of the backoffice/database.
/// </summary>
public sealed class FrontDoorCredentialsOptions
{
    /// <summary>
    /// Gets or sets the Directory (tenant) ID of the Microsoft Entra ID app registration.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Application (client) ID of the Microsoft Entra ID app registration.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client secret created for the Microsoft Entra ID app registration.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;
}
