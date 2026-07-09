using Azure;
using Azure.Identity;
using Microsoft.Extensions.Options;
using Umbraco.Automate.Core.Actions;
using Umbraco.Community.FrontDoorCache.Automate.Connections;
using Umbraco.Community.FrontDoorCache.Automate.Internal;
using Umbraco.Community.FrontDoorCache.Automate.Settings;

namespace Umbraco.Community.FrontDoorCache.Automate.Actions;

/// <summary>
/// Purges one or more URL paths from the Azure Front Door cache.
/// </summary>
[Action("frontDoorCache.purgePaths", "Purge Front Door Path(s)",
    Description = "Purges one or more URL paths from the Azure Front Door cache.",
    Group = "Azure",
    Icon = "icon-broom",
    ConnectionTypeAlias = "frontDoorCache")]
public sealed class PurgeFrontDoorPathsAction : ActionBase<PurgeFrontDoorPathsSettings, PurgeFrontDoorPathsOutput>
{
    private readonly IOptions<FrontDoorCredentialsOptions> _credentials;

    public PurgeFrontDoorPathsAction(ActionInfrastructure infrastructure, IOptions<FrontDoorCredentialsOptions> credentials)
        : base(infrastructure)
    {
        _credentials = credentials;
    }

    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<PurgeFrontDoorPathsSettings>();
        var paths = ParsePaths(settings.Paths);

        if (paths.Length == 0)
        {
            return ActionResult.Failed(
                new ArgumentException("At least one path must be supplied."),
                StepRunErrorCategory.Validation);
        }

        var connection = context.Connection
            ?? throw new InvalidOperationException("A Front Door connection is required.");
        var connectionSettings = connection.GetSettings<FrontDoorConnectionSettings>();

        try
        {
            await FrontDoorClientFactory.PurgeAsync(connectionSettings, _credentials.Value, paths, cancellationToken);
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

        return Success(new PurgeFrontDoorPathsOutput { PurgedPathCount = paths.Length, Paths = paths });
    }

    private static string[] ParsePaths(string raw) =>
        raw.Split(['\r', '\n', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
