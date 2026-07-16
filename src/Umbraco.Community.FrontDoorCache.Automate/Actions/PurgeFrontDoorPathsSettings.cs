using Umbraco.Automate.Core.Settings;

namespace Umbraco.Community.FrontDoorCache.Automate.Actions;

/// <summary>
/// Settings for the <see cref="PurgeFrontDoorPathsAction"/>.
/// </summary>
public sealed class PurgeFrontDoorPathsSettings
{
    /// <summary>
    /// Gets or sets the key of the content item whose URLs should be purged.
    /// </summary>
    [Field(
        Label = "Content Key",
        Description = "The key of the content item to purge, e.g. ${trigger.content.key}. Every URL for the item (across all cultures) is resolved and purged.",
        SupportsBindings = true,
        SortOrder = 10)]
    public string ContentKey { get; set; } = string.Empty;
}
