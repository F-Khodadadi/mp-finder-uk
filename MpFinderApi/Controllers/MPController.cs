// STEP 1: Add these using statements at the top of the file
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace MpFinderApi.Controllers
{
    [ApiController]
    [Route("MP")]
    [Produces("application/json")]
    public class MPController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public MPController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        [HttpGet("{postcode}")]
        public async Task<IActionResult> GetMPByPostcode(string postcode)
        {
            var apiKey = _configuration["TheyWorkForYou:ApiKey"];
            var encodedPostcode = Uri.EscapeDataString(postcode);
            var url = $"https://www.theyworkforyou.com/api/getMP?key={apiKey}&postcode={encodedPostcode}&output=js";

            try
            {
                var json = await _httpClient.GetStringAsync(url);
                return Ok(json);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { error = "API request failed", detail = ex.Message });
            }
        }
    }
}