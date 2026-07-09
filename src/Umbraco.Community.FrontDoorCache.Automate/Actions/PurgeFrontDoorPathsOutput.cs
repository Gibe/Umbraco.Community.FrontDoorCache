namespace Umbraco.Community.FrontDoorCache.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="PurgeFrontDoorPathsAction"/>.
/// </summary>
public sealed class PurgeFrontDoorPathsOutput
{
    /// <summary>
    /// Gets the number of paths that were submitted for purging.
    /// </summary>
    public int PurgedPathCount { get; init; }

    /// <summary>
    /// Gets the paths that were submitted for purging.
    /// </summary>
    public string[] Paths { get; init; } = [];
}
