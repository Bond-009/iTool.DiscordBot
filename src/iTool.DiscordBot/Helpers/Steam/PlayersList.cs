using System.Collections.Generic;
using Newtonsoft.Json;

namespace iTool.DiscordBot.Steam
{
    internal class PlayerList<T> where T : Player
    {
        [JsonProperty("players")]
        [JsonRequired]
        public IEnumerable<T> Players { get; set; }
    }
}
