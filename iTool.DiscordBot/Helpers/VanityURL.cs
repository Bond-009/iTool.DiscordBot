using System.Xml.Serialization;

namespace iTool.DiscordBot
{
    [XmlRoot("response")]
    public class VanityURL
    {
        [XmlElement("steamid", IsNullable = true)]
        public long? SteamID64 { get; set; }

        [XmlElement("success")]
        public int Succes { get; set; }
    }
}
