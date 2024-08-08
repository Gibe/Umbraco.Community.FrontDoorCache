using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Umbraco.Community.FrontDoorCache
{
    public class FrontDoorCacheComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddFrontDoorCache();
        }
    }
}
