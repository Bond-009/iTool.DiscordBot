using System.ComponentModel.DataAnnotations;

namespace iTool.DiscordBot
{
    public class DbEntity
    {
        [Key]
        public ulong ID { get; set; }
    }
}
