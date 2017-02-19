using System.Xml.Serialization;

namespace iTool.DiscordBot
{
    [XmlRootAttribute("response")]
    public class VanityURL
    {
        [XmlElementAttribute("steamid", IsNullable = true)]
        public long? SteamID64 { get; set; }

        [XmlElementAttribute("success")]
        public int Succes { get; set; }
    }
}
