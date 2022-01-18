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
    }
}
