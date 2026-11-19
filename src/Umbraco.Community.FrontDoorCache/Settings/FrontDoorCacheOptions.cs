using System.Security.Cryptography.X509Certificates;

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
        public AuthenticationType AuthenticationType { get; set; }
        public string CertificatePath { get; set; }
        public string CertificatePassword { get; set; }
        public string CertificateThumbprint { get; set; }
        public StoreLocation CertificateStoreLocation { get; set; } = StoreLocation.CurrentUser;
        public StoreName CertificateStoreName { get; set; } = StoreName.My;
        public string[] Domains { get; set; }
    }
}
