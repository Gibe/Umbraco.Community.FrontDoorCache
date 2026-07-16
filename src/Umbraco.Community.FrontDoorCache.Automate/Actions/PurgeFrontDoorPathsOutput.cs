namespace Umbraco.Community.FrontDoorCache.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="PurgeFrontDoorPathsAction"/>.
/// </summary>
public sealed class PurgeFrontDoorPathsOutput
{
    /// <summary>
    /// Gets the number of URLs that were resolved for the content item and submitted for purging.
    /// </summary>
    public int PurgedPathCount { get; init; }

    /// <summary>
    /// Gets the paths that were resolved for the content item and submitted for purging.
    /// </summary>
    public string[] Paths { get; init; } = [];
}
