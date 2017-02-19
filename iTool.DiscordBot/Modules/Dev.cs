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
            if ((await Context.Client.GetApplicationInfoAsync()).Owner.Id != Context.User.Id)
            { return; }
            
            System.GC.Collect();
        }
    }
}
