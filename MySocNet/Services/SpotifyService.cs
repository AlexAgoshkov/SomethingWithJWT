using Microsoft.Extensions.Configuration;
using MySocNet.Migrations;
using MySocNet.Models;
using MySocNet.Models.Spotify;
using MySocNet.Models.Spotify.User;
using MySocNet.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public class SpotifyService : ISpotifyService
    {
        private readonly IConfiguration _config;
        private readonly IRepository<User> _userRepository; 
 
        public SpotifyService(IConfiguration config, 
            IRepository<User> userRepository)
        {
            _config = config;
            _userRepository = userRepository;
        }

        public async Task<string> AddToPlaylist(string token, string playlistId, IEnumerable<string> tracks)
        {
            Uri url = new Uri("https://api.spotify.com/v1/playlists/" + playlistId + "/tracks");
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
           
            var content = new TrackInput
            {
                Uris = tracks
            };

            string json = JsonConvert.SerializeObject(content);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = authHeader;
                var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                var playlistResponse = await response.Content.ReadAsStringAsync();
                return playlistResponse;
            }
        }

        public async Task<SpotifyPlaylistResponse> CreatePlaylist(string token)
        {

            Uri url = new Uri("https://api.spotify.com/v1/users/93gnv3cvg6lfn5ofpnwpe1wpo/playlists");
            var authHeader = new AuthenticationHeaderValue("Bearer", token);

            var playlist = new SpotifyPlaylistInput
            {
                Name = DateTime.Now.ToShortDateString(),
                Description = "Your Recently Tracks",
                Public = true
            };

            string json = JsonConvert.SerializeObject(playlist);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = authHeader;

                var response = await client.PostAsync(
                    url,
                     new StringContent(json, Encoding.UTF8, "application/json"));

                var playlistResponse = await response.Content.ReadAsStringAsync();
                var playlistInfo = JsonConvert.DeserializeObject<SpotifyPlaylistResponse>(playlistResponse);
                return playlistInfo;
            }
        }
            
        public async Task<SpotifyUser> GetSpotifyUser(string spotifyLogin, User user)
        {
            Uri url = new Uri("https://api.spotify.com/v1/users/" + spotifyLogin);
            var token = await GetSpotifyTokenAsync();
            var authHeader = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = authHeader;

                var response = await client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                var spotifyUser = JsonConvert.DeserializeObject<SpotifyUser>(json);
                user.SpotifyProfile = spotifyUser;
                await _userRepository.UpdateAsync(user);
                return spotifyUser;
            }
        }

        public async Task<TokenResponse> GetSpotifyTokenAsync()
        {
            Uri url = new Uri("https://accounts.spotify.com/api/token");
            var clientid = _config["Spotify:ClientId"];
            var clientsecret = _config["Spotify:ClientSecret"];

            var encodeClientsecret = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", clientid, clientsecret)));

            var authHeader = new AuthenticationHeaderValue("Basic", encodeClientsecret);
            
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = authHeader;

                var response = await client.PostAsync(url, 
                    new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded"));
                var token = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TokenResponse>(token);
            }
        }

        public async Task<IEnumerable<string>> GetRecentlyList(string token)
        {
            Uri url = new Uri("https://api.spotify.com/v1/me/player/recently-played?type=track&limit=50");
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
           
            var spotifyTrack = await response.Content.ReadAsStringAsync();
            var traks = JsonConvert.DeserializeObject<SpotifyRecently>(spotifyTrack);

            return traks.Items.Select(x => x.Track.Uri);
        }
    }
}
