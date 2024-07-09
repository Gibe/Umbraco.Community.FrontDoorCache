using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.HealthChecks;
using Umbraco.Community.FrontDoorCache.Api;
using Umbraco.Community.FrontDoorCache.Settings;

namespace Umbraco.Community.FrontDoorCache.HealthChecks
{
    [HealthCheck(HealthCheckId, HealthCheckName, Description = "Checks the Azure Front Door API to ensure it is available.", Group = "CDN")]
    public class FrontDoorCacheHealthCheck : HealthCheck
    {
        private const string HealthCheckId = "3f0bbea9-d82e-4805-b9c0-aae4c8bf96eb";
        private const string HealthCheckName = "Front Door Cache Health Check";

        private readonly IFrontDoorApiClient _client;
        private readonly FrontDoorCacheOptions _options;

        public FrontDoorCacheHealthCheck(IOptions<FrontDoorCacheOptions> options, IFrontDoorApiClient client)
        {
            _options = options.Value;
            this._client = client;
        }

        public override async Task<IEnumerable<HealthCheckStatus>> GetStatus()
        {
            if (!_options.Enabled)
            {
                return new[]
                {
                new HealthCheckStatus("Front Door Cache is not enabled.")
                {
                    Description = "Enable Front Door Cache in settings.",
                    ResultType = StatusResultType.Info
                }
            };
            }

            var statuses = new List<HealthCheckStatus>();

            if (_options.Mode == PurgeCacheMode.Unknown)
            {
                statuses.Add(new HealthCheckStatus("Front Door Cache Mode is not set.")
                {
                    Description = "Set Front Door Cache Mode in settings.",
                    ResultType = StatusResultType.Error
                });
            }

            if (String.IsNullOrEmpty(_options.ClientId) ||
                String.IsNullOrEmpty(_options.ClientSecret) ||
                String.IsNullOrEmpty(_options.TenantId))
            {
                statuses.Add(new HealthCheckStatus("Microsoft Entra Id app registration configuration missing.")
                {
                    Description = "Set Front Door Client ID, Client Secret and Tenant ID in settings.",
                    ResultType = StatusResultType.Error
                });
            }

            var response = await _client.CheckStatus(); // TODO : Give some error information
            if (!response)
            {
                var description = "Check your Front Door credentials are correct.";

                statuses.Add(new HealthCheckStatus("Front Door API is not available.")
                {
                    Description = description,
                    ResultType = StatusResultType.Error
                });
                return statuses;
            }

            if (!statuses.Any())
            {
                statuses.Add(new HealthCheckStatus("Front Door API is available.")
                {
                    ResultType = StatusResultType.Success
                });
            }

            return statuses;
        }

        public override HealthCheckStatus ExecuteAction(HealthCheckAction action) => new("How did you get here?")
        {
            ResultType = StatusResultType.Info
        };
    }
}
