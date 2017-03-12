using System.Xml.Serialization;

namespace iTool.DiscordBot.Steam
{
    [XmlRoot("response")]
    public class VanityURL
    {
        [XmlElement("steamid", IsNullable = true)]
        public ulong? SteamID64 { get; set; }

        [XmlElement("success")]
        public int Succes { get; set; }
    }
}
