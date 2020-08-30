using Microsoft.Extensions.Configuration;
using MySocNet.Migrations;
using MySocNet.Models.Spotify;
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

        public SpotifyService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> GetSpotifyTokenAsync() //TODO: Refactoring 
        {
            string url = "https://accounts.spotify.com/api/token";
            var clientid = _config["Spotify:ClientId"];
            var clientsecret = _config["Spotify:ClientSecret"];

            var encodeClientsecret = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", clientid, clientsecret)));

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Accept = "application/json";
            webRequest.Headers.Add("Authorization: Basic " + encodeClientsecret);
            
          
            var request = ("grant_type=client_credentials");
            byte[] req_bytes = Encoding.ASCII.GetBytes(request);
            webRequest.ContentLength = req_bytes.Length;

            using (Stream strm = webRequest.GetRequestStream())
            {
                await strm.WriteAsync(req_bytes, 0, req_bytes.Length);
                      strm.Close();
            }

            HttpWebResponse resp = (HttpWebResponse)await webRequest.GetResponseAsync();
            string json = "";
            using (Stream respStr = resp.GetResponseStream())
            {
                using (StreamReader rdr = new StreamReader(respStr, Encoding.UTF8))
                {
                    json = await rdr.ReadToEndAsync();
                    rdr.Close();
                }
            }
            return json;
        }

        public async Task<List<string>> GetRecentlyList(string token)
        {
            Uri url = new Uri("https://api.spotify.com/v1/me/player/recently-played?type=track&limit=10");
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

            return traks.Items.Select(x => x.Track.Uri).ToList();
        }

        public async Task CreatePlaylist(string token)
        {
            Uri url = new Uri("https://api.spotify.com/v1/users/93gnv3cvg6lfn5ofpnwpe1wpo/playlists");
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            var postData = "name:" + Uri.EscapeDataString("New Playlist");
            postData += ",description:" + Uri.EscapeDataString("New playlist description");
            postData += ",public:" + Uri.EscapeDataString("true");
            var data = Encoding.ASCII.GetBytes(postData);
            //HttpContent content = new HttpContent();

            string json = JsonConvert.SerializeObject(postData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = authHeader;
            
            var response = await client.PostAsync(url, content);
           
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var finalRequestUri = response.RequestMessage.RequestUri;

                if (finalRequestUri != url)
                {
                    response = await client.PostAsync(finalRequestUri, content);
                }
            }
            var responseString = await response.Content.ReadAsStringAsync();
        }
    }
}
