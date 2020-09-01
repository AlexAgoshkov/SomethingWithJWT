using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySocNet.Models;
using MySocNet.Models.Spotify;
using MySocNet.Models.Spotify.User;
using MySocNet.Services.Interfaces;
using Newtonsoft.Json;

namespace MySocNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WeatherForecastController : ApiControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public WeatherForecastController(IRepository<User> userRepository, 
            IHttpClientFactory clientFactory) : base(userRepository)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet("GetWeather")]
        public async Task<IActionResult> GetWeather(string token)
        {
            Uri url = new Uri("https://api.spotify.com/v1/users/93gnv3cvg6lfn5ofpnwpe1wpo");
            var authHeader = new AuthenticationHeaderValue("Bearer", token);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = authHeader;

            var response = await client.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var finalRequestUri = response.RequestMessage.RequestUri;

                if (finalRequestUri != url)
                {
                    response = await client.GetAsync(finalRequestUri);
                }
            }
 
            var spotifyUser = await response.Content.ReadAsStringAsync();
            return JsonResult(JsonConvert.DeserializeObject<SpotifyUser>(spotifyUser));
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var client = _clientFactory.CreateClient();
            HttpRequestMessage request = GetRequest("https://jsonplaceholder.typicode.com/users");

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStringAsync();
                var root = JsonConvert.DeserializeObject<IEnumerable<RootUsers>>(responseStream);
                return JsonResult(root);
            }
            return BadRequest();
        }

        private static HttpRequestMessage GetRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "");
            return request;
        }
    }
}