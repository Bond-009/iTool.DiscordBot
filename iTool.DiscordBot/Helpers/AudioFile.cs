using System.Collections.Generic;
using System.Xml.Serialization;

namespace iTool.DiscordBot
{
    public class AudioFile
    {
        [XmlAttribute("FileName")]
        public string FileName { get; set; } = string.Empty;

        [XmlArray("Names")]
        [XmlArrayItem("Name")]
        public List<string> Names { get; set; } = new List<string>();
    }
}
