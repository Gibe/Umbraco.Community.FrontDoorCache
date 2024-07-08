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
using Umbraco.Extensions;

namespace Umbraco.Community.FrontDoorCache
{
	public class MediaSavedNotificationHandler
			: INotificationAsyncHandler<MediaSavedNotification>
	{
		private readonly ILogger _logger;
		private readonly FrontDoorCacheOptions _options;
		private readonly IFrontDoorApiClient _frontDoorApiClient;
		private readonly IPublishedUrlProvider _publishedUrlProvider;
		private readonly IUmbracoContextFactory _umbracoContextFactory;

		public MediaSavedNotificationHandler(
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

		public async Task HandleAsync(MediaSavedNotification notification, CancellationToken cancellationToken)
		{
			if (!_options.Enabled)
			{
				_logger.LogInformation("FrontDoor cache purging is disabled");
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
				case PurgeCacheMode.SelfAndAncestors:
				{
					await PurgeMedia(notification.SavedEntities);
					return;
				}
				case PurgeCacheMode.Unknown:
					_logger.LogError("FrontDoor cache mode is unknown");
					return;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private async Task PurgeMedia(IEnumerable<IMedia> savedMedia)
		{
			using var ctx = _umbracoContextFactory.EnsureUmbracoContext();
			var mediaCache = ctx.UmbracoContext.Media;
			if (mediaCache == null)
			{
				_logger.LogError("Failed to get published media cache");
				return;
			}

			var publishedMedia = new List<IPublishedContent>();
			foreach (var saved in savedMedia)
			{
				var media = mediaCache.GetById(saved.Id);
				if (media == null)
				{
					_logger.LogWarning("Failed to get published media with id {Id}", saved.Id);
					continue;
				}

				publishedMedia.Add(media);
			}

			var request = GetContentPathsToPurge(publishedMedia);
			var paths = String.Join(", ", request.ContentPaths);
			_logger.LogInformation("Purging FrontDoor cache {Mode}, clearing paths {paths}", _options.Mode, paths);
			await _frontDoorApiClient.SendPurgeRequest(request);
		}

		private async Task PurgeAll()
		{
			_logger.LogInformation("Purging all FrontDoor cache");
			var result = await _frontDoorApiClient.SendPurgeAllRequest();
			if (!result)
			{
				_logger.LogError("Failed to purge FrontDoor cache");
			}
			else
			{
				_logger.LogInformation("Successfully purged FrontDoor cache");
			}
		}

		private FrontDoorPurgeContent GetContentPathsToPurge(List<IPublishedContent> publishedMedia)
		{
			var contentPaths = new List<string>();
			
			foreach (var media in publishedMedia)
			{
				foreach (var mediaCulture in media.Cultures)
				{
					var url = media.MediaUrl(_publishedUrlProvider, mediaCulture.Key, UrlMode.Relative);
					var uri = new UriBuilder(url);
					uri.Path = string.Join("/", uri.Path.Split("/").SkipLast());
					contentPaths.Add(uri.Path + "/*");
				}
			}

			return new FrontDoorPurgeContent(contentPaths);
		}
	}

}