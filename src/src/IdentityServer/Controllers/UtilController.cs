using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using IdentityServer.Attributes;
using IdentityServer.Settings;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace IdentityServer.Controllers
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class UtilController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IOptions<AzureKeyVaultCert> _options;

        public UtilController(IConfiguration configuration, IOptions<AzureKeyVaultCert> options)
        {
            _configuration = configuration;
            _options = options;
        }

        public IActionResult Index()
        {
            return Ok();
        }

        public IActionResult GenerateKeyForClient([FromQuery] string pass) 
        {
            if (string.IsNullOrEmpty(pass))
            {
                return Ok();
            }

            var secret = new Secret(pass.Sha256());
            return Ok(new { success = true, secret = secret.Value });
        }

        public async Task<IActionResult> TestCert()
        {
            var azKeyVaultCert = _options.Value;
            CertificateClientOptions options = new CertificateClientOptions
            {
                Retry =
                {
                    Delay= TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 5,
                    Mode = RetryMode.Exponential
                }
            };
            CertificateClient client = new CertificateClient(new Uri(azKeyVaultCert.KeyVaultUri), new DefaultAzureCredential(), options);
            using X509Certificate2 certificate = await client.DownloadCertificateAsync(azKeyVaultCert.CertName);

            ECDsa key = certificate.GetECDsaPrivateKey();

            ECDsaSecurityKey ecdsaCertificatePublicKey = new ECDsaSecurityKey(key);
            return Ok(
                new { 
                    success = true, 
                    notAfter = certificate.NotAfter,
                    issuerName = certificate.IssuerName,
                    signatureAlgorithm = key.SignatureAlgorithm,
                    keySize = key.KeySize
                }
            );
        }
    }
}
