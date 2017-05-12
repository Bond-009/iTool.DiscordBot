using Newtonsoft.Json;

namespace iTool.DiscordBot.Steam
{
    public class Stat
    {
        [JsonProperty("name")]
        [JsonRequired]
        public string Name { get; set; }

        [JsonProperty("value")]
        [JsonRequired]
        public int Value { get; set; }
    }
}
