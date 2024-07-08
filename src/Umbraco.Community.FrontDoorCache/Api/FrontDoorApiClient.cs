using Azure;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Cdn;
using Azure.ResourceManager.Cdn.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Umbraco.Community.FrontDoorCache.Api
{
	public class FrontDoorApiClient : IFrontDoorApiClient
	{
		private ILogger<FrontDoorApiClient> _logger;
		private FrontDoorCacheOptions _options;

		public FrontDoorApiClient(IOptions<FrontDoorCacheOptions> options, ILogger<FrontDoorApiClient> logger)
		{
			_options = options.Value;
			_logger = logger;
		}

		public async Task<bool> CheckStatus()
		{
			var credential = new ClientSecretCredential(_options.TenantId, _options.ClientId, _options.ClientSecret);
			var client = new ArmClient(credential);

			var frontDoorResourceIdentifier = FrontDoorEndpointResource.CreateResourceIdentifier(_options.SubscriptionId,
				_options.ResourceGroupName, _options.FrontDoorName, _options.EndpointName);
			var endpoint = client.GetFrontDoorEndpointResource(frontDoorResourceIdentifier);

			var fd = await endpoint.GetAsync();
			return fd.HasValue;
		}

		public async Task<bool> SendPurgeRequest(FrontDoorPurgeContent content)
		{
			var credential = new ClientSecretCredential(_options.TenantId, _options.ClientId, _options.ClientSecret);
			var client = new ArmClient(credential);

			var frontDoorResourceIdentifier = FrontDoorEndpointResource.CreateResourceIdentifier(_options.SubscriptionId,
				_options.ResourceGroupName, _options.FrontDoorName, _options.EndpointName);
			var endpoint = client.GetFrontDoorEndpointResource(frontDoorResourceIdentifier);

			foreach (var domain in _options.Domains)
			{
				content.Domains.Add(domain);
			}
			
			await endpoint.PurgeContentAsync(WaitUntil.Started, content);
			return true;
		}

		public async Task<bool> SendPurgeAllRequest()
		{
			return await SendPurgeRequest(new FrontDoorPurgeContent(new[] {"*"}));
		}
	}
}
