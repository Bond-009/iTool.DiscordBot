using System.Xml.Serialization;

namespace iTool.DiscordBot.Steam
{
    public class Player
    {
        [XmlElement("steamid")]
        public ulong SteamID { get; set; }
    }
}
