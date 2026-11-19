using System.Security.Cryptography.X509Certificates;
using Azure.Core;
using Azure.Identity;
using Umbraco.Community.FrontDoorCache.Settings;

namespace Umbraco.Community.FrontDoorCache.Api
{
    internal static class FrontDoorCredentialFactory
    {
        public static TokenCredential Create(FrontDoorCacheOptions options)
        {
            return options.AuthenticationType switch
            {
                AuthenticationType.ManagedIdentity => string.IsNullOrEmpty(options.ClientId)
                    ? new ManagedIdentityCredential()
                    : new ManagedIdentityCredential(options.ClientId),
                AuthenticationType.Certificate => new ClientCertificateCredential(options.TenantId, options.ClientId, LoadCertificate(options)),
                _ => new ClientSecretCredential(options.TenantId, options.ClientId, options.ClientSecret)
            };
        }

        private static X509Certificate2 LoadCertificate(FrontDoorCacheOptions options)
        {
            if (!string.IsNullOrEmpty(options.CertificatePath))
            {
                return string.IsNullOrEmpty(options.CertificatePassword)
                    ? new X509Certificate2(options.CertificatePath)
                    : new X509Certificate2(options.CertificatePath, options.CertificatePassword);
            }

            using var store = new X509Store(options.CertificateStoreName, options.CertificateStoreLocation);
            store.Open(OpenFlags.ReadOnly);

            var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, options.CertificateThumbprint, false);
            if (certificates.Count == 0)
            {
                throw new InvalidOperationException($"Certificate with thumbprint '{options.CertificateThumbprint}' was not found in {options.CertificateStoreLocation}/{options.CertificateStoreName}.");
            }

            return certificates[0];
        }
    }
}
