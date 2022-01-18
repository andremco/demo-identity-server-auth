using Azure.Storage.Files.Shares;
using IdentityServer.Attributes;
using IdentityServer.Settings;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _environment;

        public UtilController(
            IConfiguration configuration, 
            IOptions<CertificateSettings> optionsCert,
            IOptions<StorageAccountSettings> optionsStorage,
            IWebHostEnvironment environment
        )
        {
            _configuration = configuration;
            _certificateSettings = optionsCert.Value;
            _storageAccountSettings = optionsStorage.Value;
            _environment = environment;
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
            //using (var fileStream = System.IO.File.Create(Path.Combine(_environment.ContentRootPath, "Test.txt")))
            //using (var memoryStream = new MemoryStream())
            //{
            //    stream.Seek(0, SeekOrigin.Begin);
            //    stream.CopyTo(fileStream);
            //}

            if (System.IO.File.Exists(Path.Combine(_environment.ContentRootPath, _certificateSettings.FileCertName)))
            {
                //var ecdsaCertificate = new X509Certificate2(Path.Combine(_environment.ContentRootPath, _certificateSettings.FileCertName), _certificateSettings.PasswordCert);
                //ECDsaSecurityKey ecdsaCertificatePublicKey = new ECDsaSecurityKey(ecdsaCertificate.GetECDsaPrivateKey());

                return Ok();
            }
            return NotFound(new { message = "Arquivo não existe " + Path.Combine(_environment.ContentRootPath, _certificateSettings.FileCertName) });
        }
    }
}
