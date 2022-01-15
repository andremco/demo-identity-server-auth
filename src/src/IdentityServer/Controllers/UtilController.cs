using IdentityServer.Attributes;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IdentityServer.Controllers
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class UtilController : Controller
    {
        private readonly IConfiguration _configuration;

        public UtilController(IConfiguration configuration)
        {
            _configuration = configuration;
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
