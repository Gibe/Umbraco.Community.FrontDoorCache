namespace Umbraco.Community.FrontDoorCache.Settings
{
    public class FrontDoorCacheOptions
    {
        public bool Enabled { get; set; }
        public PurgeCacheMode Mode { get; set; }
        public string SubscriptionId { get; set; }
        public string ResourceGroupName { get; set; }
        public string FrontDoorName { get; set; }
        public string EndpointName { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string[] Domains { get; set; }
    }
}
