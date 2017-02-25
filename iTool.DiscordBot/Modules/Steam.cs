using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Steam : ModuleBase
    {
        [Command("resolvevanityurl")]
        [Summary("Returns the steamID64 of the user")]
        public async Task ResolveVanityURL(string name = null)
        {
            if (string.IsNullOrEmpty(Program.Settings.SteamKey))
            {
                await Program.Log(new LogMessage(LogSeverity.Warning, "", "No SteamKey found."));
                return;
            }

            if (name == null) { name = Context.User.Username; }

            await ReplyAsync((await DiscordBot.Steam.ResolveVanityURL(name)).ToString());
        }

        [Command("steamprofile")]
        [Summary("Returns the steamprofile of the user")]
        public async Task SteamProfile(string name = null)
        {
            if (string.IsNullOrEmpty(Program.Settings.SteamKey))
            {
                await Program.Log(new LogMessage(LogSeverity.Warning, "", "No SteamKey found."));
                return;
            }

            if (name == null) { name = Context.User.Username; }

            await ReplyAsync("https://steamcommunity.com/profiles/" + (await DiscordBot.Steam.ResolveVanityURL(name)).ToString());
        }
    }
}
