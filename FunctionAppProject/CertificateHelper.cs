
using System;
using System.Security.Cryptography.X509Certificates;

namespace Company.Function
{
    public static class CertificateHelper
    {
        public static X509Certificate2 FindByThumbprint(
            string thumbprint,
            StoreName storeName,
            StoreLocation storeLocation)
        {
            var certificateStore = new X509Store(storeName, storeLocation);
            certificateStore.Open(OpenFlags.ReadOnly);

            var certCollection = certificateStore.Certificates.Find(
                                        X509FindType.FindByThumbprint,
                                        thumbprint,
                                        false);

            certificateStore.Close();

            if (certCollection.Count > 0)
            {
                X509Certificate2 cert = certCollection[0];
                return cert;
            }

            throw new ArgumentException(
                string.Format("Cannot find certificate with thumbprint {0} in certificate store ", thumbprint));
        }
    }
}