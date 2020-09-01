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
        [Route("AddSpotifyToUser")]
        public async Task<IActionResult> GetSpotifyUser(string login)
        {
            var user = await CurrentUser();
            var result = await _spotifyService.GetSpotifyUser(login, user);
            return JsonResult(result);        
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
            var playlist = await _spotifyService.CreatePlaylist(token);
            return JsonResult(playlist);
        }

        [HttpPost]
        [Route("MakePlaylistFromRecently")]
        public async Task<IActionResult> MakePlaylistFromRecently(string token)
        {
            var tracks = await _spotifyService.GetRecentlyList(token);
            var playlist = await _spotifyService.CreatePlaylist(token);
            return JsonResult(await _spotifyService.AddToPlaylist(token, playlist.Id, tracks));
        }
    }
}