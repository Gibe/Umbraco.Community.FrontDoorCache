using Azure;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Cdn;
using Azure.ResourceManager.Cdn.Models;
using Umbraco.Community.FrontDoorCache.Automate.Connections;
using Umbraco.Community.FrontDoorCache.Automate.Settings;

namespace Umbraco.Community.FrontDoorCache.Automate.Internal;

/// <summary>
/// Builds Azure Front Door management clients from a connection's endpoint settings and the
/// site-configured Entra ID credentials, and performs purge/status calls against them.
/// Not part of the public API surface.
/// </summary>
internal static class FrontDoorClientFactory
{
    public static FrontDoorEndpointResource GetEndpoint(FrontDoorConnectionSettings settings, FrontDoorCredentialsOptions credentials)
    {
        var credential = new ClientSecretCredential(credentials.TenantId, credentials.ClientId, credentials.ClientSecret);
        var client = new ArmClient(credential);

        var frontDoorResourceIdentifier = FrontDoorEndpointResource.CreateResourceIdentifier(
            settings.SubscriptionId, settings.ResourceGroupName, settings.FrontDoorName, settings.EndpointName);

        return client.GetFrontDoorEndpointResource(frontDoorResourceIdentifier);
    }

    public static async Task PurgeAsync(
        FrontDoorConnectionSettings settings,
        FrontDoorCredentialsOptions credentials,
        IEnumerable<string> contentPaths,
        CancellationToken cancellationToken)
    {
        var endpoint = GetEndpoint(settings, credentials);
        var content = new FrontDoorPurgeContent(contentPaths);
        await endpoint.PurgeContentAsync(WaitUntil.Started, content, cancellationToken);
    }
}
