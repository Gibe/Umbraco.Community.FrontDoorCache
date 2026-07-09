using Umbraco.Automate.Core.Settings;

namespace Umbraco.Community.FrontDoorCache.Automate.Actions;

/// <summary>
/// Settings for the <see cref="PurgeFrontDoorPathsAction"/>.
/// </summary>
public sealed class PurgeFrontDoorPathsSettings
{
    /// <summary>
    /// Gets or sets the paths to purge, one per line (or comma-separated).
    /// </summary>
    [Field(
        Label = "Paths",
        Description = "One path per line (or comma-separated), e.g. /blog/my-post or /images/*. Supports runtime bindings such as ${trigger.url}.",
        SupportsBindings = true,
        SortOrder = 10)]
    public string Paths { get; set; } = string.Empty;
}
