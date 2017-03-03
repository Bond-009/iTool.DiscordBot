using System.Xml.Serialization;

namespace iTool.DiscordBot.Steam
{
    public class PlayerBan
    {
        [XmlElement("SteamId")]
        public ulong SteamID { get; set; }
        public bool CommunityBanned { get; set; }
        public bool VACBanned { get; set; }
        public int NumberOfVACBans { get; set; }
        public int DaysSinceLastBan { get; set; }
        public int NumberOfGameBans { get; set; }
        public string EconomyBan { get; set; }
    }
}
