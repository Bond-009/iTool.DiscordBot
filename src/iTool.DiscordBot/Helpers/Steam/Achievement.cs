using Newtonsoft.Json;

namespace iTool.DiscordBot.Steam
{
    public class Achievement
    {
        [JsonProperty("name")]
        [JsonRequired]
        public string Name { get; set; }

        [JsonProperty("achieved")]
        [JsonRequired]
        public int Achieved { get; set; }
    }
}
