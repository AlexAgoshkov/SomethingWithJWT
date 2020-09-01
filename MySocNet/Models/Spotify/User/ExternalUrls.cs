using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Models.Spotify.User
{
    public class ExternalUrls
    {
        [Key]
        public int Id { get; set; }

        [JsonProperty("spotify")]
        public string Spotify { get; set; }
    }
}
