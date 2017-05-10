using System.Collections.Generic;

namespace iTool.DiscordBot
{
    public class AudioFile
    {
        public string FileName { get; set; }
        public ICollection<string> Names { get; set; }
    }
}
