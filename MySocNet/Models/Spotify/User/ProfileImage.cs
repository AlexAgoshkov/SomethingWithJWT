using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Models.Spotify.User
{
    public class ProfileImage
    {
        [Key]
        public int Id { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }
    }
}
