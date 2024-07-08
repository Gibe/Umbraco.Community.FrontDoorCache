using Azure.ResourceManager.Cdn.Models;

namespace Umbraco.Community.FrontDoorCache.Api
{
	public interface IFrontDoorApiClient
	{
		Task<bool> CheckStatus();
		Task<bool> SendPurgeRequest(FrontDoorPurgeContent content);
		Task<bool> SendPurgeAllRequest();
	}
}
