using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Community.FrontDoorCache.Api;
using Umbraco.Community.FrontDoorCache.HealthChecks;
using Umbraco.Community.FrontDoorCache.Settings;

namespace Umbraco.Community.FrontDoorCache
{
    public static class UmbracoBuilderExtensions
	{
		public static void AddFrontDoorCache(this IUmbracoBuilder builder)
		{
			builder.HealthChecks().Add<FrontDoorCacheHealthCheck>();

			builder.Services.AddTransient<IFrontDoorApiClient, FrontDoorApiClient>();

			builder.Services
				.AddOptions<FrontDoorCacheOptions>()
				.BindConfiguration("FrontDoor:Cache");

			builder.AddNotificationAsyncHandler<MediaSavedNotification, MediaSavedNotificationHandler>();
			builder.AddNotificationAsyncHandler<ContentSavedNotification, ContentSavedNotificationHandler>();
		}
	}
}
