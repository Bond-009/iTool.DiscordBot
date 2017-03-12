using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Dev : ModuleBase
    {
        [Command("gc")]
        [Alias("collectgarbage")]
        [Summary("Forces the GC to clean up resources")]
        public async Task GC()
        {
            if (!Utils.IsTrustedUser(Context.User))
            { return; }

            System.GC.Collect();

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = "Blacklist",
                Color = new Color(3, 144, 255),
                Description = ":thumbsup:"
            });
        }
    }
}
