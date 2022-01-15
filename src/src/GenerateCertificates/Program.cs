using CertificateManager;
using GenerateCertificates.Certs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace GenerateCertificates
{
    /*
        Article - CREATE CERTIFICATES FOR IDENTITYSERVER4 SIGNING USING .NET CORE
        https://damienbod.com/2020/02/10/create-certificates-for-identityserver4-signing-using-net-core/
    */
    class Program
    {
        static CreateCertificates _cc;

        static void Main(string[] args)
        {
            var sp = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            _cc = sp.GetService<CreateCertificates>();

            var rsaCert = RsaCertificate.Create(_cc, "localhost", 10);
            var ecdsaCert = ECDsaCertificate.Create(_cc, "localhost", 10);

            string password = "MyPass@123";
            var iec = sp.GetService<ImportExportCertificate>();

            var rsaCertPfxBytes =
                iec.ExportSelfSignedCertificatePfx(password, rsaCert);
            File.WriteAllBytes("rsaCert.pfx", rsaCertPfxBytes);

            Console.WriteLine("Generated file - rsaCert.pfx");

            var ecdsaCertPfxBytes =
                iec.ExportSelfSignedCertificatePfx(password, ecdsaCert);
            File.WriteAllBytes("ecdsaCert.pfx", ecdsaCertPfxBytes);

            Console.WriteLine("Generated file - ecdsaCert.pfx");

            Console.ReadKey();
        }
    }
}
