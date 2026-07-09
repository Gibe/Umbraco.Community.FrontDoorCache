using Umbraco.Automate.Core.Settings;

namespace Umbraco.Community.FrontDoorCache.Automate.Actions;

/// <summary>
/// Settings for the <see cref="GetContentOrMediaUrlAction"/>. Supply exactly one of
/// <see cref="ContentKey"/> or <see cref="MediaKey"/>.
/// </summary>
public sealed class GetContentOrMediaUrlSettings
{
    /// <summary>
    /// Gets or sets the key of a content item to resolve a URL for.
    /// </summary>
    [Field(
        Label = "Content Key",
        Description = "The key of a content item to resolve a URL for, e.g. ${trigger.content.key}. Leave blank when resolving a media URL.",
        SupportsBindings = true,
        SortOrder = 10)]
    public string? ContentKey { get; set; }

    /// <summary>
    /// Gets or sets the key of a media item to resolve a URL for.
    /// </summary>
    [Field(
        Label = "Media Key",
        Description = "The key of a media item to resolve a URL for, e.g. ${trigger.media.key}. Leave blank when resolving a content URL.",
        SupportsBindings = true,
        SortOrder = 20)]
    public string? MediaKey { get; set; }

    /// <summary>
    /// Gets or sets the culture to resolve the URL for. Leave blank for the default/invariant culture.
    /// </summary>
    [Field(
        Label = "Culture",
        Description = "Optional culture to resolve the URL for (e.g. en-US). Leave blank for the default/invariant culture.",
        SupportsBindings = true,
        SortOrder = 30)]
    public string? Culture { get; set; }
}
