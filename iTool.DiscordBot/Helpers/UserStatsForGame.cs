using System.Collections.Generic;
using System.Xml.Serialization;

namespace iTool.DiscordBot
{
    [XmlRoot("playerstats")]
    public class UserStatsForGame
    {
        [XmlElement("steamID")]
        public long SteamID { get; set; }

        [XmlElement("gameName")]
        public string GameName { get; set; }

        [XmlArray("stats")]
        [XmlArrayItem("stat")]
        public List<Stat> Stats { get; set; } = new List<Stat>();

        [XmlArray("achievements")]
        [XmlArrayItem("achievement")]
        public List<Achievement> Achievements { get; set; } = new List<Achievement>();
    }
}
