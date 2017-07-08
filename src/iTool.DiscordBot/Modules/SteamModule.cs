using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;

namespace iTool.DiscordBot.Modules
{
    public class SteamModule : ModuleBase
    {
        private readonly Settings _settings;
        private readonly ISteamUser _steamUser;

        public SteamModule(Settings settings, ISteamUser steamUser)
        {
            _settings = settings;
            _steamUser = steamUser;
        }

        [Command("vanityurl")]
        [Alias("resolvevanityurl")]
        [Summary("Returns the steamID64 of the user")]
        public async Task ResolveVanityURL(string name = null)
        {
            if (name == null) { name = Context.User.Username; }

            await ReplyAsync(((await _steamUser.ResolveVanityUrlAsync(name)).Data).ToString());
        }

        [Command("steam")]
        [Summary("Returns basic steam profile information")]
        public async Task PlayerSummaries(string name = null)
        {
            if (name == null) { name = Context.User.Username; }

            PlayerSummaryModel player = (await _steamUser.GetPlayerSummaryAsync(
                                            (await _steamUser.ResolveVanityUrlAsync(name)).Data)
                                        ).Data;

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Player summary for {player.Nickname}",
                Color = _settings.GetColor(),
                ThumbnailUrl = player.AvatarMediumUrl,
                Url = player.ProfileUrl
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "SteamID";
                f.Value = player.SteamId;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Nickname";
                f.Value = player.Nickname;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Persona state";
                f.Value = (PersonaState)Enum.ToObject(typeof(PersonaState) , player.ProfileState);
            }));
        }

        [Command("playerbans")]
        [Alias("getplayerbans")]
        [Summary("Returns Community, VAC, and Economy ban statuses for given players")]
        public async Task PlayerBans(string name = null)
        {
            if (name == null) { name = Context.User.Username; }

            PlayerBansModel players = (await _steamUser.GetPlayerBansAsync(
                                            (await _steamUser.ResolveVanityUrlAsync(name)).Data)
                                        ).Data.FirstOrDefault();

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Community, VAC, and Economy ban statuses",
                Color = _settings.GetColor(),
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "SteamID";
                f.Value = players.SteamId;
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

            await ReplyAsync("https://steamcommunity.com/profiles/" + (await _steamUser.ResolveVanityUrlAsync(name)).Data);
        }
    }
}
