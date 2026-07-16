namespace Umbraco.Community.FrontDoorCache.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="PurgeFrontDoorAllAction"/>.
/// </summary>
public sealed class PurgeFrontDoorAllOutput
{
    /// <summary>
    /// Gets a value indicating whether the purge request was accepted by Front Door.
    /// </summary>
    public bool Accepted { get; init; }
}
