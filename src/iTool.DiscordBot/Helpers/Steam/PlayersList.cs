using System.Collections.Generic;
using System.Xml.Serialization;

namespace iTool.DiscordBot.Steam
{
    [XmlRoot("response")]
    public class PlayerList<T> where T : Player
    {
        [XmlArray("players")]
        [XmlArrayItem("player")]
        public IEnumerable<T> Players { get; set; }
    }
}
