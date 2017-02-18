using Discord.Commands;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Steam : ModuleBase
    {
        [Command("resolvevanityurl")]
        [Summary("Returns the steamID64 of the player")]
        public async Task ResolveVanityURL(string name = null)
        {
            if (name == null) { name = Context.User.Username; }

            await ReplyAsync((await DiscordBot.Steam.ResolveVanityURL(name)).ToString());
        }
    }
}
