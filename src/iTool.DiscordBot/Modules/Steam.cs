using Discord;
using Discord.Commands;
using iTool.DiscordBot.Steam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Steam : ModuleBase
    {
        Settings settings;
        SteamAPI client;

        public Steam(Settings settings, SteamAPI steamapi)
        {
            if (string.IsNullOrEmpty(settings.SteamKey))
            {
                throw new Exception("No SteamKey found.");
            }

            this.settings = settings;
            this.client = steamapi;
        }

        [Command("vanityurl")]
        [Alias("resolvevanityurl")]
        [Summary("Returns the steamID64 of the user")]
        public async Task ResolveVanityURL(string name = null)
        {
            if (name == null) { name = Context.User.Username; }

            await ReplyAsync((await client.ResolveVanityURL(name)).ToString());
        }

        [Command("steam")]
        [Alias("getplayersummaries", "playersummaries")]
        [Summary("Returns basic steam profile information")]
        public async Task PlayerSummaries(string name = null)
        {
            if (name == null) { name = Context.User.Username; }
            PlayerSummary player = (await client.GetPlayerSummaries(new [] {(await client.ResolveVanityURL(name))})).First();

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Player summary fot {player.PersonaName}",
                Color = settings.GetColor(),
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

            PlayerBan players = (await client.GetPlayerBans(new [] {(await client.ResolveVanityURL(name))})).First();

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Community, VAC, and Economy ban statuses",
                Color = settings.GetColor(),
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

            await ReplyAsync("https://steamcommunity.com/profiles/" + (await client.ResolveVanityURL(name)).ToString());
        }
    }
}
