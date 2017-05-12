using Newtonsoft.Json;

namespace iTool.DiscordBot.Steam
{
    internal interface Player
    {
        [JsonProperty("steamid")]
        [JsonRequired]
        ulong SteamID { get; set; }
    }
}
