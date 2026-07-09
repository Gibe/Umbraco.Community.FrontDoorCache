using Umbraco.Automate.Core.Actions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;

namespace Umbraco.Community.FrontDoorCache.Automate.Actions;

/// <summary>
/// Resolves the absolute URL for a content or media item, for use with Purge Front Door Path(s).
/// Does not call Front Door and requires no connection.
/// </summary>
[Action("frontDoorCache.getContentOrMediaUrl", "Get Content or Media URL",
    Description = "Resolves the absolute URL for a content or media item, ready to feed into Purge Front Door Path(s).",
    Group = "Content",
    Icon = "icon-link")]
public sealed class GetContentOrMediaUrlAction : ActionBase<GetContentOrMediaUrlSettings, GetContentOrMediaUrlOutput>
{
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    private readonly IPublishedUrlProvider _publishedUrlProvider;

    public GetContentOrMediaUrlAction(
        ActionInfrastructure infrastructure,
        IUmbracoContextFactory umbracoContextFactory,
        IPublishedUrlProvider publishedUrlProvider)
        : base(infrastructure)
    {
        _umbracoContextFactory = umbracoContextFactory;
        _publishedUrlProvider = publishedUrlProvider;
    }

    public override Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<GetContentOrMediaUrlSettings>();

        var hasContentKey = !string.IsNullOrWhiteSpace(settings.ContentKey);
        var hasMediaKey = !string.IsNullOrWhiteSpace(settings.MediaKey);

        if (hasContentKey == hasMediaKey)
        {
            return Task.FromResult(ActionResult.Failed(
                new ArgumentException("Supply exactly one of Content Key or Media Key."),
                StepRunErrorCategory.Validation));
        }

        var rawKey = hasContentKey ? settings.ContentKey : settings.MediaKey;
        if (!Guid.TryParse(rawKey, out var key))
        {
            return Task.FromResult(ActionResult.Failed(
                new ArgumentException($"'{rawKey}' is not a valid key."),
                StepRunErrorCategory.Validation));
        }

        var culture = string.IsNullOrWhiteSpace(settings.Culture) ? null : settings.Culture;

        // Automate runs actions on a background workflow engine, not inside a live HTTP request,
        // so IPublishedUrlProvider has no ambient UmbracoContext to attach to without this.
        using var umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext();

        var url = hasContentKey
            ? _publishedUrlProvider.GetUrl(key, UrlMode.Absolute, culture)
            : _publishedUrlProvider.GetMediaUrl(key, UrlMode.Absolute, culture);

        // GetUrl/GetMediaUrl return a "#"-prefixed sentinel (or string.Empty for media) when the
        // item can't be found or isn't routable/published - not an absolute URL either way.
        if (string.IsNullOrEmpty(url) || url.StartsWith('#'))
        {
            return Task.FromResult(ActionResult.Failed(
                new InvalidOperationException("Could not resolve a URL for the specified item. It may not exist, or may not be published."),
                StepRunErrorCategory.InvalidResponse));
        }

        var path = new UriBuilder(url).Path;

        return Task.FromResult(Success(new GetContentOrMediaUrlOutput { Url = url, Path = path }));
    }
}
