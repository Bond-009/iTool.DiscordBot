using System.Collections.Generic;
using System.Xml.Serialization;

namespace iTool.DiscordBot.Steam
{
    [XmlRoot("response")]
    public class PlayerSummaries
    {
        [XmlArray("players")]
        [XmlArrayItem("player")]
        public List<Player> Players { get; set; } = new List<Player>();
    }
}
