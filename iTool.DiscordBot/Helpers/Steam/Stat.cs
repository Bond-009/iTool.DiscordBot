using System.Xml.Serialization;

namespace iTool.DiscordBot.Steam
{
    public class Stat
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("value")]
        public int Value { get; set; }
    }
}
