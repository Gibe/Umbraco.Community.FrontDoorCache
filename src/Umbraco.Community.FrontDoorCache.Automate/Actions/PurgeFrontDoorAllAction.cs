using Azure;
using Azure.Identity;
using Microsoft.Extensions.Options;
using Umbraco.Automate.Core.Actions;
using Umbraco.Community.FrontDoorCache.Automate.Connections;
using Umbraco.Community.FrontDoorCache.Automate.Internal;
using Umbraco.Community.FrontDoorCache.Automate.Settings;

namespace Umbraco.Community.FrontDoorCache.Automate.Actions;

/// <summary>
/// Purges the entire Azure Front Door cache for the configured endpoint.
/// </summary>
[Action("frontDoorCache.purgeAll", "Purge Front Door (All)",
    Description = "Purges the entire Azure Front Door cache for this endpoint.",
    Group = "Azure",
    Icon = "icon-trash",
    ConnectionTypeAlias = "frontDoorCache")]
public sealed class PurgeFrontDoorAllAction : ActionBase<PurgeFrontDoorAllSettings, PurgeFrontDoorAllOutput>
{
    private readonly IOptions<FrontDoorCredentialsOptions> _credentials;

    public PurgeFrontDoorAllAction(ActionInfrastructure infrastructure, IOptions<FrontDoorCredentialsOptions> credentials)
        : base(infrastructure)
    {
        _credentials = credentials;
    }

    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var connection = context.Connection
            ?? throw new InvalidOperationException("A Front Door connection is required.");
        var connectionSettings = connection.GetSettings<FrontDoorConnectionSettings>();

        try
        {
            await FrontDoorClientFactory.PurgeAsync(connectionSettings, _credentials.Value, ["/*"], cancellationToken);
        }
        catch (AuthenticationFailedException ex)
        {
            return ActionResult.Failed(ex, StepRunErrorCategory.Authentication);
        }
        catch (RequestFailedException ex) when (ex.Status is 401 or 403)
        {
            return ActionResult.Failed(ex, StepRunErrorCategory.Authentication);
        }
        catch (RequestFailedException ex) when (ex.Status == 429)
        {
            return ActionResult.Failed(ex, StepRunErrorCategory.RateLimiting);
        }
        catch (RequestFailedException ex)
        {
            return ActionResult.Failed(ex, StepRunErrorCategory.ServiceUnavailable);
        }
        catch (Exception ex)
        {
            return ActionResult.Failed(ex, StepRunErrorCategory.Unknown);
        }

        return Success(new PurgeFrontDoorAllOutput { Accepted = true });
    }
}
