using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Models.Spotify
{
    public class SpotifyAccount
    {
        [Key]
        public int Id { get; set; }

        public string SpotifyLogin { get; set; }

        public ICollection<SpotifyTrack> SpotifyTracks { get; set; }
    }
}
