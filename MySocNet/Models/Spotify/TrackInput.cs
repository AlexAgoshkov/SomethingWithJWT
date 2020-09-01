using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Models.Spotify
{
    public class TrackInput
    {
        [JsonProperty("uris")]
        public IEnumerable<string> Uris { get; set; }
    }
}
