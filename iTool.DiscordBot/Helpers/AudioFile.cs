using System.Collections.Generic;

namespace iTool.DiscordBot
{
    public class AudioFile
    {
        public string FileName { get; set; } = string.Empty;
        public List<string> Names { get; set; } = new List<string>();
    }
}
