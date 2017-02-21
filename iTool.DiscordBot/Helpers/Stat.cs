using System.Xml.Serialization;

namespace iTool.DiscordBot
{
    public class Stat
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("value")]
        public int Value { get; set; }
    }
}
