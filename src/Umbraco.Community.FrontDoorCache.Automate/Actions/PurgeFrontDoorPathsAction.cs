using Azure;
using Azure.Identity;
using Microsoft.Extensions.Options;
using Umbraco.Automate.Core.Actions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Community.FrontDoorCache.Automate.Connections;
using Umbraco.Community.FrontDoorCache.Automate.Internal;
using Umbraco.Community.FrontDoorCache.Automate.Settings;
using Umbraco.Extensions;

namespace Umbraco.Community.FrontDoorCache.Automate.Actions;

/// <summary>
/// Resolves every URL for a content item (across all of its cultures) and purges them from the
/// Azure Front Door cache.
/// </summary>
[Action("frontDoorCache.purgePaths", "Purge Front Door Path(s)",
    Description = "Resolves every URL for a content item and purges them from the Azure Front Door cache.",
    Group = "Azure",
    Icon = "icon-broom",
    ConnectionTypeAlias = "frontDoorCache")]
public sealed class PurgeFrontDoorPathsAction : ActionBase<PurgeFrontDoorPathsSettings, PurgeFrontDoorPathsOutput>
{
    private readonly IOptions<FrontDoorCredentialsOptions> _credentials;
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    private readonly IPublishedUrlProvider _publishedUrlProvider;

    public PurgeFrontDoorPathsAction(
        ActionInfrastructure infrastructure,
        IOptions<FrontDoorCredentialsOptions> credentials,
        IUmbracoContextFactory umbracoContextFactory,
        IPublishedUrlProvider publishedUrlProvider)
        : base(infrastructure)
    {
        _credentials = credentials;
        _umbracoContextFactory = umbracoContextFactory;
        _publishedUrlProvider = publishedUrlProvider;
    }

    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<PurgeFrontDoorPathsSettings>();

        if (!Guid.TryParse(settings.ContentKey, out var contentKey))
        {
            return ActionResult.Failed(
                new ArgumentException($"'{settings.ContentKey}' is not a valid content key."),
                StepRunErrorCategory.Validation);
        }

        var paths = ResolvePaths(contentKey);
        if (paths.Length == 0)
        {
            return ActionResult.Failed(
                new InvalidOperationException("Could not resolve any URLs for the specified content item. It may not exist, or may not be published."),
                StepRunErrorCategory.InvalidResponse);
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

    private string[] ResolvePaths(Guid contentKey)
    {
        // Automate runs actions on a background workflow engine, not inside a live HTTP request,
        // so the published content cache has no ambient UmbracoContext to attach to without this.
        using var umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext();

        var content = umbracoContextReference.UmbracoContext.Content?.GetById(contentKey);
        if (content == null)
        {
            return [];
        }

        var paths = new List<string>();
        foreach (var cultureKey in content.Cultures.Keys)
        {
            // An empty culture key means invariant content; passing null gives us the right URL for it.
            var culture = string.IsNullOrEmpty(cultureKey) ? null : cultureKey;
            var url = content.Url(_publishedUrlProvider, culture, UrlMode.Absolute);

            // content.Url returns "#" for any content without a URL, for legacy reasons.
            if (string.IsNullOrEmpty(url) || url == "#")
            {
                continue;
            }

            paths.Add(new UriBuilder(url).Path);
        }

        return paths.Distinct().ToArray();
    }
}
