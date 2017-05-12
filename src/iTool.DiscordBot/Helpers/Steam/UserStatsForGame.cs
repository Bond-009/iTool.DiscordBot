using Newtonsoft.Json;
using System.Collections.Generic;

namespace iTool.DiscordBot.Steam
{
    public class UserStatsForGameResponse : SteamResponse<UserStatsForGame>
    {
        [JsonProperty("playerstats")]
        [JsonRequired]
        public override UserStatsForGame Data { get; set; }
    }

    public class UserStatsForGame
    {
        [JsonProperty("steamID")]
        [JsonRequired]
        public ulong SteamID { get; set; }

        [JsonProperty("gameName")]
        [JsonRequired]
        public string GameName { get; set; }

        [JsonProperty("stats")]
        [JsonRequired]
        public IEnumerable<Stat> Stats { get; set; }

        [JsonProperty("achievements")]
        [JsonRequired]
        public IEnumerable<Achievement> Achievements { get; set; }
    }
}
