using Discord;
using Discord.Commands;
using iTool.DiscordBot.Steam;
using System.Linq;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Steam : ModuleBase
    {
        [Command("vanityurl")]
        [Alias("resolvevanityurl")]
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
        [Alias("getplayersummaries", "playersummaries")]
        [Summary("Returns basic steam profile information")]
        public async Task PlayerSummaries(string name = null)
        {
            if (string.IsNullOrEmpty(Program.Settings.SteamKey))
            {
                await Program.Log(new LogMessage(LogSeverity.Warning, "", "No SteamKey found."));
                return;
            }

            if (name == null) { name = Context.User.Username; }
            PlayerSummaries player = await DiscordBot.Steam.Steam.GetPlayerSummaries(new [] {(await DiscordBot.Steam.Steam.ResolveVanityURL(name))});

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Player summary fot {player.Players.First().PersonaName}",
                Color = new Color((uint)Program.Settings.Color),
                ThumbnailUrl = player.Players.First().AvatarMedium,
                Url = player.Players.First().ProfileURL
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "SteamID";
                f.Value = player.Players.First().SteamID;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Persona name";
                f.Value = player.Players.First().PersonaName;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Persona state";
                f.Value = player.Players.First().PersonaState.ToString();
            }));
        }

        [Command("playerbans")]
        [Alias("getplayerbans")]
        [Summary("Returns Community, VAC, and Economy ban statuses for given players")]
        public async Task PlayerBans(string name = null)
        {
            if (string.IsNullOrEmpty(Program.Settings.SteamKey))
            {
                await Program.Log(new LogMessage(LogSeverity.Warning, "", "No SteamKey found."));
                return;
            }

            if (name == null) { name = Context.User.Username; }

            PlayerBans player = await DiscordBot.Steam.Steam.GetPlayerBans(new [] {(await DiscordBot.Steam.Steam.ResolveVanityURL(name))});

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Community, VAC, and Economy ban statuses",
                Color = new Color((uint)Program.Settings.Color),
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "SteamID";
                f.Value = player.Players.First().SteamID;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "CommunityBanned";
                f.Value = player.Players.First().CommunityBanned;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "VACBanned";
                f.Value = player.Players.First().VACBanned;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Number of VAC bans";
                f.Value = player.Players.First().NumberOfVACBans;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Days since last ban";
                f.Value = player.Players.First().DaysSinceLastBan;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Number of game bans";
                f.Value = player.Players.First().NumberOfGameBans;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Economy ban";
                f.Value = player.Players.First().EconomyBan;
            }));
        }

        [Command("steamprofile")]
        [Summary("Returns the URL to the steam profile of the user")]
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
