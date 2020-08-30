using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySocNet.Models;
using MySocNet.Services.Interfaces;

namespace MySocNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SpotifyController : ApiControllerBase
    {
        private readonly ISpotifyService _spotifyService;

        public SpotifyController(
            ISpotifyService spotifyService,
            IRepository<User> userRepository
            ) : base(userRepository)
        {
            _spotifyService = spotifyService;
        }


        [HttpGet]
        [Route("GetSpotifyToken")]
        public async Task<IActionResult> GetSpotifyToken()
        {
           var token = await _spotifyService.GetSpotifyTokenAsync();
           return JsonResult(token);
        }

        [HttpGet]
        [Route("GetRecentlyTracks")]
        public async Task<IActionResult> GetRecentlyTracks(string token)
        {
            var result = await _spotifyService.GetRecentlyList(token);
            return JsonResult(result);
        }

        [HttpPost]
        [Route("CreatePlaylist")]
        public async Task<IActionResult> CreatePlaylist(string token)
        {
             await _spotifyService.CreatePlaylist(token);
            return Ok();
        }
    }
}