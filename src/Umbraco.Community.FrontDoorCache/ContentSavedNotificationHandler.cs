using Azure.ResourceManager.Cdn.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Community.FrontDoorCache.Api;
using Umbraco.Community.FrontDoorCache.Culture;
using Umbraco.Community.FrontDoorCache.Settings;
using Umbraco.Extensions;

namespace Umbraco.Community.FrontDoorCache
{
    public class ContentSavedNotificationHandler
      : INotificationAsyncHandler<ContentSavedNotification>
    {
        private readonly ILogger _logger;
        private readonly FrontDoorCacheOptions _options;
        private readonly IFrontDoorApiClient _frontDoorApiClient;
        private readonly IPublishedUrlProvider _publishedUrlProvider;
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public ContentSavedNotificationHandler(
        IOptions<FrontDoorCacheOptions> options,
        IUmbracoContextFactory umbracoContextFactory,
        IPublishedUrlProvider publishedUrlProvider,
        IFrontDoorApiClient frontDoorApiClient,
        ILogger<MediaSavedNotificationHandler> logger)
        {
            _options = options.Value;
            _frontDoorApiClient = frontDoorApiClient;
            _umbracoContextFactory = umbracoContextFactory;
            _publishedUrlProvider = publishedUrlProvider;
            _logger = logger;
        }

        public async Task HandleAsync(ContentSavedNotification notification, CancellationToken cancellationToken)
        {
            if (!_options.Enabled)
            {
                _logger.LogInformation("Front Door cache purging is disabled");
                return;
            }

            switch (_options.Mode)
            {
                case PurgeCacheMode.All:
                {
                    await PurgeAll();
                    return;
                }
                case PurgeCacheMode.Self:
                {
                    await PurgeContent(notification.SavedEntities);
                    return;
                }
                case PurgeCacheMode.SelfAndAncestors:
                {
                    await PurgeContent(notification.SavedEntities, true);
                    return;
                }
                case PurgeCacheMode.Unknown:
                    _logger.LogError("Front Door cache mode is unknown");
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task PurgeContent(IEnumerable<IContent> savedContent, bool includeAncestors = false)
        {
            using var ctx = _umbracoContextFactory.EnsureUmbracoContext();
            var contentCache = ctx.UmbracoContext.Content;
            if (contentCache == null)
            {
                _logger.LogError("Failed to get published content cache");
                return;
            }

            var publishedContent = new List<IPublishedContent>();
            foreach (var saved in savedContent)
            {
                var content = contentCache.GetById(saved.Id);
                if (content == null)
                {
                    _logger.LogWarning("Failed to get published content with id {Id}", saved.Id);
                    continue;
                }

                if (content.IsDraft())
                {
                    continue;
                }
                publishedContent.Add(content);
                if (includeAncestors)
                {
                    while (content.Parent != null)
                    {
                        if (publishedContent.Any(c => c.Id == content.Parent.Id))
                        {
                            break;
                        }

                        content = content.Parent;
                        publishedContent.Add(content);
                    }
                }
            }

            var request = GetContentPathsToPurge(publishedContent);
            var paths = string.Join(", ", request.ContentPaths);
            _logger.LogInformation("Purging Front Door cache {Mode}, clearing paths {paths}", _options.Mode, paths);
            await _frontDoorApiClient.SendPurgeRequest(request);
        }

        private async Task PurgeAll()
        {
            _logger.LogInformation("Purging all Front Door cache");
            if (await _frontDoorApiClient.SendPurgeAllRequest())
            {
                _logger.LogInformation("Successfully purged Front Door cache");
            }
            else
            {
                _logger.LogError("Failed to purge Front Door cache");
            }
        }

        private FrontDoorPurgeContent GetContentPathsToPurge(List<IPublishedContent> publishedContent)
        {
            var contentPaths = new List<string>();

            foreach (var content in publishedContent)
            {
                foreach (var culture in content.Cultures)
                {
                    // If the culture name is empty, null will give us what we want
                    var url = content.Url(_publishedUrlProvider, culture.CultureKeyOrNull(), UrlMode.Absolute);

                    // content.Url returns '#' for any content without an URL for legacy purposes
                    if (url.Equals("#"))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        var uri = new UriBuilder(url);
                        contentPaths.Add(uri.Path);
                    }
                }
            }

            return new FrontDoorPurgeContent(contentPaths);
        }

    }
}
