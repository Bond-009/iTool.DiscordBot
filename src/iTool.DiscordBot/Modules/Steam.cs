using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using iTool.DiscordBot.Steam;

namespace iTool.DiscordBot.Modules
{
    public class Steam : ModuleBase
    {
        private Settings _settings;
        private readonly SteamAPI _client;

        public Steam(Settings settings, SteamAPI steamapi)
        {
            _settings = settings;
            _client = steamapi;
        }

        protected override void BeforeExecute()
        {
            if (string.IsNullOrEmpty(_settings.SteamKey))
            {
                throw new Exception("No SteamKey found.");
            }
        }

        [Command("vanityurl")]
        [Alias("resolvevanityurl")]
        [Summary("Returns the steamID64 of the user")]
        public async Task ResolveVanityURL(string name = null)
        {
            if (name == null) { name = Context.User.Username; }

            await ReplyAsync((await _client.ResolveVanityURL(name)).ToString());
        }

        [Command("steam")]
        [Alias("getplayersummaries", "playersummaries")]
        [Summary("Returns basic steam profile information")]
        public async Task PlayerSummaries(string name = null)
        {
            if (name == null) { name = Context.User.Username; }
            PlayerSummary player = (await _client.GetPlayerSummaries(new [] {(await _client.ResolveVanityURL(name))})).First();

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Player summary fot {player.PersonaName}",
                Color = _settings.GetColor(),
                ThumbnailUrl = player.AvatarMedium,
                Url = player.ProfileURL
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "SteamID";
                f.Value = player.SteamID;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Persona name";
                f.Value = player.PersonaName;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Persona state";
                f.Value = player.PersonaState.ToString();
            }));
        }

        [Command("playerbans")]
        [Alias("getplayerbans")]
        [Summary("Returns Community, VAC, and Economy ban statuses for given players")]
        public async Task PlayerBans(string name = null)
        {
            if (name == null) { name = Context.User.Username; }

            PlayerBan players = (await _client.GetPlayerBans(new [] {(await _client.ResolveVanityURL(name))})).First();

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Community, VAC, and Economy ban statuses",
                Color = _settings.GetColor(),
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "SteamID";
                f.Value = players.SteamID;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "CommunityBanned";
                f.Value = players.CommunityBanned;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "VACBanned";
                f.Value = players.VACBanned;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Number of VAC bans";
                f.Value = players.NumberOfVACBans;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Days since last ban";
                f.Value = players.DaysSinceLastBan;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Number of game bans";
                f.Value = players.NumberOfGameBans;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Economy ban";
                f.Value = players.EconomyBan;
            }));
        }

        [Command("steamprofile")]
        [Summary("Returns the URL to the steam profile of the user")]
        public async Task SteamProfile(string name = null)
        {
            if (name == null) { name = Context.User.Username; }

            await ReplyAsync("https://steamcommunity.com/profiles/" + (await _client.ResolveVanityURL(name)).ToString());
        }
    }
}
