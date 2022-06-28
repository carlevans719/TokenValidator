using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using System;
using System.Linq;
namespace TokenValidator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValidatorController : ControllerBase
    {

        private readonly ILogger<ValidatorController> _logger;

        public ValidatorController(ILogger<ValidatorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            if (Request.Headers.TryGetValue("api-key", out var apiKey))
            {
                // compare to api key from env file
                if (apiKey.First().Equals("abc-123"))
                {
                    Console.WriteLine("API Key Validation Successful");
                    Response.StatusCode = 200;
                    return Ok();
                }
                else
                {
                    Console.WriteLine("API Key Validation Failed");
                    Response.StatusCode = 403;
                    return StatusCode(StatusCodes.Status403Forbidden);
                }                
            }
            else if (Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                string[] parts = authHeader.First().Split(" ");
                if (!String.Equals(parts[0], "bearer", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Invalid Scheme");
                    Response.StatusCode = 403;
                    return StatusCode(StatusCodes.Status403Forbidden);
                }

                var validationResult = new JsonWebTokenHandler()
                        .ValidateToken(parts[1], TokenValidationParamsStore.GetParams());

                if (validationResult.IsValid)
                {
                    Console.WriteLine("JWT Valid");
                    Response.StatusCode = 200;
                    return Ok();
                }
                else
                {
                    IdentityModelEventSource.ShowPII = true;
                    Console.WriteLine(validationResult.Exception);
                    Console.WriteLine("JWT Invalid");
                    Response.StatusCode = 403;
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
            else
            {
                Response.StatusCode = 401;
                return Unauthorized();
            }
        }
    }
}
