using Azure.Storage.Files.Shares;
using IdentityServer.Attributes;
using IdentityServer.Settings;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace IdentityServer.Controllers
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class UtilController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly CertificateSettings _certificateSettings;
        private readonly StorageAccountSettings _storageAccountSettings;

        public UtilController(
            IConfiguration configuration, 
            IOptions<CertificateSettings> optionsCert,
            IOptions<StorageAccountSettings> optionsStorage
        )
        {
            _configuration = configuration;
            _certificateSettings = optionsCert.Value;
            _storageAccountSettings = optionsStorage.Value;
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

        public IActionResult TestCert()
        {
            ShareClient share = new ShareClient(_storageAccountSettings.ConnectionString, _storageAccountSettings.ShareName);

            ShareDirectoryClient directory = share.GetDirectoryClient(_storageAccountSettings.Folder);

            var file = directory.GetFileClient(_certificateSettings.FileCertName);

            Stream stream = file.OpenRead();

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                var ecdsaCertificate = new X509Certificate2(memoryStream.ToArray(), _certificateSettings.PasswordCert,  X509KeyStorageFlags.PersistKeySet
                             | X509KeyStorageFlags.Exportable);

                ECDsaSecurityKey ecdsaCertificatePublicKey = new ECDsaSecurityKey(ecdsaCertificate.GetECDsaPrivateKey());
            }

            return Ok();
        }
    }
}
