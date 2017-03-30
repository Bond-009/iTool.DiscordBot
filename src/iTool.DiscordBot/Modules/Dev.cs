using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Dev : ModuleBase
    {
        DependencyMap depMap;

        public Dev(DependencyMap map) => this.depMap = map;

        [Command("gc")]
        [Alias("collectgarbage")]
        [Summary("Forces the GC to clean up resources")]
        [RequireTrustedUser]
        public async Task GC()
        {
            System.GC.Collect();

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = "GC",
                Color = new Color((uint)depMap.Get<Settings>().Color),
                Description = ":thumbsup:"
            });
        }
    }
}
