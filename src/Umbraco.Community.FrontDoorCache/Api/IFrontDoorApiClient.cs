using Azure.ResourceManager.Cdn.Models;

namespace Umbraco.Community.FrontDoorCache.Api
{
	/// <summary>
	/// Api Client for purging a Standard/Premium Front Door endpoint
	/// </summary>
	public interface IFrontDoorApiClient
	{
		/// <summary>
		/// Makes a basic request to the Front Door API to check if connection is working
		/// </summary>
		/// <returns>true if connection is successful</returns>
		Task<bool> CheckStatus();

		/// <summary>
		/// Send a request to purge a set of content from a Front Door endpoint
		/// </summary>
		/// <param name="content">The content to purge</param>
		/// <returns>true if successful</returns>
		Task<bool> SendPurgeRequest(FrontDoorPurgeContent content);

		/// <summary>
		/// Send a request to purge all content from a Front Door endpoint
		/// </summary>
		/// <returns>true if successful</returns>
		Task<bool> SendPurgeAllRequest();
	}
}
