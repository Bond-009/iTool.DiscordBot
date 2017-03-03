using System.Collections.Generic;
using System.Xml.Serialization;

namespace iTool.DiscordBot
{
    public class AudioIndex
    {
        [XmlArray("AudioFiles")]
        [XmlArrayItem("AudioFile")]
        public List<AudioFile> AudioFiles { get; set; } = new List<AudioFile>();
    }
}
