using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Models.Spotify
{
    public class SpotifyTrack
    {
        [Key]
        public int Id { get; set; }

        public string TrackUri { get; set; }

        public string TrackLink { get; set; }
    }
}
