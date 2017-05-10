using System;
using System.Collections.Generic;

namespace iTool.DiscordBot
{
    public class Tag : DbEntity
    {
        public ulong AuthorID { get; set; }
        public ulong GuildID { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Attachment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
