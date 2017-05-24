using System;
using System.ComponentModel.DataAnnotations;

namespace iTool.DiscordBot
{
    public class Tag : DbEntity
    {
        [Required]
        public ulong AuthorID { get; set; }
        [Required]
        public ulong GuildID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Text { get; set; }
        public string Attachment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
