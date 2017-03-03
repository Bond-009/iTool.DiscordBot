using Discord;
using Discord.Commands;
using iTool.DiscordBot.Steam;
using System.Linq;
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
            
            await ReplyAsync((await DiscordBot.Steam.Steam.ResolveVanityURL(name)).ToString());
        }

        [Command("steam")]
        [Alias("GetPlayerSummaries", "PlayerSummaries")]
        [Summary("Returns the steamID64 of the user")]
        public async Task PlayerSummaries(string name = null)
        {
            if (string.IsNullOrEmpty(Program.Settings.SteamKey))
            {
                await Program.Log(new LogMessage(LogSeverity.Warning, "", "No SteamKey found."));
                return;
            }

            if (name == null) { name = Context.User.Username; }
            PlayerSummaries player = await DiscordBot.Steam.Steam.GetPlayerSummaries(new [] {(await DiscordBot.Steam.Steam.ResolveVanityURL(name)).ToString()});
            
            EmbedBuilder b = new EmbedBuilder()
            {
                Title = $"Player summary fot {player.Players.First().PersonaName}",
                Color = new Color(3, 144, 255),
                ThumbnailUrl = player.Players.First().AvatarMedium,
                Url = player.Players.First().ProfileURL
            };
            b.AddField(f =>
            {
                f.IsInline = true;
                f.Name = "SteamID";
                f.Value = player.Players.First().SteamID;
            });
            b.AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Persona name";
                f.Value = player.Players.First().PersonaName;
            });
            b.AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Persona state";
                f.Value = player.Players.First().PersonaState.ToString();
            });
            
            await ReplyAsync("", embed: b);
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

            await ReplyAsync("https://steamcommunity.com/profiles/" + (await DiscordBot.Steam.Steam.ResolveVanityURL(name)).ToString());
        }
    }
}
