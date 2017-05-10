using System.Collections.Generic;
using System.Xml.Serialization;

namespace iTool.DiscordBot.Steam
{
    [XmlRoot("playerstats")]
    public class UserStatsForGame
    {
        [XmlElement("steamID")]
        public ulong SteamID { get; set; }

        [XmlElement("gameName")]
        public string GameName { get; set; }

        [XmlArray("stats")]
        [XmlArrayItem("stat")]
        public IEnumerable<Stat> Stats { get; set; }

        [XmlArray("achievements")]
        [XmlArrayItem("achievement")]
        public IEnumerable<Achievement> Achievements { get; set; }
    }
}
