using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySocNet.Models;
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
        public async Task<IActionResult> GetWeather()
        {
            HttpClient client = _clientFactory.CreateClient();

            var response = await client.GetAsync("http://api.openweathermap.org/data/2.5/weather?q=Kyiv&appid=dc8dc3940b7e3cae682cec03ffb80bcf");

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStringAsync();
                Root root = JsonConvert.DeserializeObject<Root>(responseStream);
                return JsonResult(root);
            }
            return BadRequest();
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
            return request;
        }
    }
}