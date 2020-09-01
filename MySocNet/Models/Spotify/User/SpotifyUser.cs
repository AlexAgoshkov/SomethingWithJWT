using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Models.Spotify.User
{
    public class SpotifyUser
    {
        [Key]
        public int UserSpotifyId { get; set; } //Entity

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("external_urls")]
        public virtual ExternalUrls ExternalUrls { get; set; }

        [JsonProperty("followers")]
        public virtual Followers Followers { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("images")]
        public virtual ICollection<ProfileImage> Images { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
