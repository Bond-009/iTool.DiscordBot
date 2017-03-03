using System.Xml.Serialization;

namespace iTool.DiscordBot.Steam
{
    public class Achievement
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("achieved")]
        public int Achieved { get; set; }
    }
}
