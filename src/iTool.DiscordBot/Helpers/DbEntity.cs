using System.ComponentModel.DataAnnotations;

namespace iTool.DiscordBot
{
    public abstract class DbEntity
    {
        [Key]
        public ulong ID { get; set; }
    }
}
