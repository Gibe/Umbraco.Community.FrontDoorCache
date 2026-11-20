using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.Cdn;
using Azure.ResourceManager.Cdn.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Community.FrontDoorCache.Settings;

namespace Umbraco.Community.FrontDoorCache.Api
{
    public class FrontDoorApiClient : IFrontDoorApiClient
    {
        private readonly ILogger<FrontDoorApiClient> _logger;
        private readonly FrontDoorCacheOptions _options;

        public FrontDoorApiClient(IOptions<FrontDoorCacheOptions> options, ILogger<FrontDoorApiClient> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task<bool> CheckStatus()
        {
            try
            {
                var endpoint = GetEndpoint();
                var fd = await endpoint.GetAsync();
                return fd.HasValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failure while checking Front Door API is available");
            }

            return false;
        }

        public async Task<bool> SendPurgeRequest(FrontDoorPurgeContent content)
        {
            foreach (var domain in _options.Domains)
            {
                content.Domains.Add(domain);
            }

            try
            {
                var endpoint = GetEndpoint();
                await endpoint.PurgeContentAsync(WaitUntil.Started, content);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failure while communicating with Front Door API");
                return false;
            }
        }

        private FrontDoorEndpointResource GetEndpoint()
        {
            var client = new ArmClient(FrontDoorCredentialFactory.Create(_options));

            var frontDoorResourceIdentifier = FrontDoorEndpointResource.CreateResourceIdentifier(_options.SubscriptionId,
              _options.ResourceGroupName, _options.FrontDoorName, _options.EndpointName);
            return client.GetFrontDoorEndpointResource(frontDoorResourceIdentifier);
        }

        public async Task<bool> SendPurgeAllRequest()
        {
            return await SendPurgeRequest(new FrontDoorPurgeContent(new[] { "/*" }));
        }
    }
}
