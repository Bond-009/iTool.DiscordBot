using Newtonsoft.Json;

namespace iTool.DiscordBot.Steam
{
    public class VanityURlResponse : SteamResponse<VanityURL>
    {
        [JsonProperty("response")]
        [JsonRequired]
        public override VanityURL Data { get; set; }
    }

    public class VanityURL
    {
        [JsonProperty("steamid")]
        public ulong? SteamID64 { get; set; }

        [JsonProperty("success")]
        [JsonRequired]
        public int Success { get; set; }
    }
}
