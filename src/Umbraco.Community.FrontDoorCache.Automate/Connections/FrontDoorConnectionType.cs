using Azure;
using Azure.Identity;
using Microsoft.Extensions.Options;
using Umbraco.Automate.Core.Connections;
using Umbraco.Community.FrontDoorCache.Automate.Internal;
using Umbraco.Community.FrontDoorCache.Automate.Settings;

namespace Umbraco.Community.FrontDoorCache.Automate.Connections;

/// <summary>
/// Connection type for an Azure Front Door Standard/Premium profile, authenticated via a
/// Microsoft Entra ID app registration configured in <c>FrontDoor:Cache:Credentials</c>.
/// </summary>
[ConnectionType("frontDoorCache", "Azure Front Door",
    Description = "Connects to an Azure Front Door Standard/Premium profile to purge cached content.",
    Group = "Azure",
    Icon = "icon-cloud")]
public sealed class FrontDoorConnectionType : ConnectionTypeBase<FrontDoorConnectionSettings>
{
    private readonly IOptions<FrontDoorCredentialsOptions> _credentials;

    public FrontDoorConnectionType(ConnectionTypeInfrastructure infrastructure, IOptions<FrontDoorCredentialsOptions> credentials)
        : base(infrastructure)
    {
        _credentials = credentials;
    }

    /// <inheritdoc />
    public override async Task<ConnectionValidationResult> ValidateAsync(
        object? settings, CancellationToken cancellationToken)
    {
        var connectionSettings = settings as FrontDoorConnectionSettings;
        var credentials = _credentials.Value;

        if (connectionSettings is null ||
            string.IsNullOrWhiteSpace(connectionSettings.SubscriptionId) ||
            string.IsNullOrWhiteSpace(connectionSettings.ResourceGroupName) ||
            string.IsNullOrWhiteSpace(connectionSettings.FrontDoorName) ||
            string.IsNullOrWhiteSpace(connectionSettings.EndpointName))
        {
            return ConnectionValidationResult.Failure("All Front Door connection fields are required.");
        }

        if (string.IsNullOrWhiteSpace(credentials.TenantId) ||
            string.IsNullOrWhiteSpace(credentials.ClientId) ||
            string.IsNullOrWhiteSpace(credentials.ClientSecret))
        {
            return ConnectionValidationResult.Failure(
                "Microsoft Entra ID credentials are not configured. Set TenantId, ClientId and ClientSecret under FrontDoor:Cache:Credentials in appsettings.json.");
        }

        try
        {
            var endpoint = FrontDoorClientFactory.GetEndpoint(connectionSettings, credentials);
            var response = await endpoint.GetAsync(cancellationToken);
            return response.HasValue
                ? ConnectionValidationResult.Success("Successfully connected to the Front Door endpoint.")
                : ConnectionValidationResult.Failure("The Front Door endpoint could not be found.");
        }
        catch (AuthenticationFailedException ex)
        {
            return ConnectionValidationResult.Failure("Could not authenticate with Microsoft Entra ID.", [ex.Message]);
        }
        catch (RequestFailedException ex)
        {
            return ConnectionValidationResult.Failure("The Front Door endpoint could not be reached.", [ex.Message]);
        }
    }
}
