using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Community.FrontDoorCache.Automate.Settings;

namespace Umbraco.Community.FrontDoorCache.Automate.Configuration;

/// <summary>
/// Binds the <c>FrontDoor:Cache:Credentials</c> configuration section. The Connection Type and
/// Actions are auto-discovered by Umbraco Automate, so this composer exists solely for this
/// options binding.
/// </summary>
public sealed class FrontDoorAutomateComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services
            .AddOptions<FrontDoorCredentialsOptions>()
            .BindConfiguration("FrontDoor:Cache:Credentials");
    }
}
