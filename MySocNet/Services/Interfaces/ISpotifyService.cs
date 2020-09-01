using MySocNet.Models;
using MySocNet.Models.Spotify;
using MySocNet.Models.Spotify.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface ISpotifyService
    {
        Task<TokenResponse> GetSpotifyTokenAsync();

        Task<IEnumerable<string>> GetRecentlyList(string token);

        Task<SpotifyPlaylistResponse> CreatePlaylist(string token);

        Task<SpotifyUser> GetSpotifyUser(string spotifyLogin, User user);

        Task<string> AddToPlaylist(string token, string playlistId, IEnumerable<string> tracks);
    }
}
