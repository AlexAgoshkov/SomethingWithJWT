using MySocNet.Models.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface ISpotifyService
    {
        Task<string> GetSpotifyTokenAsync();

        Task<List<string>> GetRecentlyList(string token);

        Task CreatePlaylist(string token);
    }
}
