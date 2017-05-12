using Newtonsoft.Json;

namespace iTool.DiscordBot.Steam
{
    public class SteamResponse<T>
    {
        [JsonProperty("response")]
        [JsonRequired]
        public virtual T Data { get; set; }
    }
}
