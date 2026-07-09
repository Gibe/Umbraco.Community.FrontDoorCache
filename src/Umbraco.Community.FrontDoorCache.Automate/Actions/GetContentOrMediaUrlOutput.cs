namespace Umbraco.Community.FrontDoorCache.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="GetContentOrMediaUrlAction"/>.
/// </summary>
public sealed class GetContentOrMediaUrlOutput
{
    /// <summary>
    /// Gets the resolved absolute URL, e.g. <c>https://www.example.com/blog/my-post</c>.
    /// </summary>
    public string Url { get; init; } = string.Empty;

    /// <summary>
    /// Gets just the path portion of the resolved URL, e.g. <c>/blog/my-post</c> - ready to feed
    /// directly into the Paths field of Purge Front Door Path(s).
    /// </summary>
    public string Path { get; init; } = string.Empty;
}
