using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Community.FrontDoorCache.Culture
{
    public static class CultureExtensions
    {
        public static string? CultureKeyOrNull(this KeyValuePair<string, PublishedCultureInfo> culture)
        {
            return string.IsNullOrEmpty(culture.Key) ? (string?)null : culture.Key;
        }
    }
}
