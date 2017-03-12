using System.Collections.Generic;
using System.Xml.Serialization;

namespace iTool.DiscordBot.Steam
{
    [XmlRoot("response")]
    public class PlayerBans
    {
        [XmlArray("players")]
        [XmlArrayItem("player")]
        public List<PlayerBan> Players { get; set; } = new List<PlayerBan>();
    }
}
