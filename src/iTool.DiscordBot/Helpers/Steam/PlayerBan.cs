using Newtonsoft.Json;

namespace iTool.DiscordBot.Steam
{
    public class PlayerBan : Player
    {
        public ulong SteamID { get; set; }
        [JsonRequired]
        public bool CommunityBanned { get; set; }
        [JsonRequired]
        public bool VACBanned { get; set; }
        [JsonRequired]
        public int NumberOfVACBans { get; set; }
        [JsonRequired]
        public int DaysSinceLastBan { get; set; }
        [JsonRequired]
        public int NumberOfGameBans { get; set; }
        [JsonRequired]
        public string EconomyBan { get; set; }
    }
}
