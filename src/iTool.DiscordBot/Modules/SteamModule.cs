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

            await ReplyAsync(((await _steamUser.ResolveVanityUrlAsync(name).ConfigureAwait(false)).Data).ToString()).ConfigureAwait(false);
        }

        [Command("steam")]
        [Summary("Returns basic steam profile information")]
        public async Task PlayerSummaries(string name = null)
        {
            PlayerSummaryModel player = (await _steamUser.GetPlayerSummaryAsync(
                                            (await _steamUser.ResolveVanityUrlAsync(name ?? Context.User.Username).ConfigureAwait(false)).Data).ConfigureAwait(false)
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
                f.Value = (PersonaState)Enum.ToObject(typeof(PersonaState), player.ProfileState);
            })
            .Build()).ConfigureAwait(false);
        }

        [Command("playerbans")]
        [Alias("getplayerbans")]
        [Summary("Returns Community, VAC, and Economy ban statuses for given players")]
        public async Task PlayerBans(string name = null)
        {
            PlayerBansModel player = (await _steamUser.GetPlayerBansAsync(
                                            (await _steamUser.ResolveVanityUrlAsync(name ?? Context.User.Username).ConfigureAwait(false)).Data).ConfigureAwait(false)
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
                f.Value = player.SteamId;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "CommunityBanned";
                f.Value = player.CommunityBanned;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "VACBanned";
                f.Value = player.VACBanned;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Number of VAC bans";
                f.Value = player.NumberOfVACBans;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Days since last ban";
                f.Value = player.DaysSinceLastBan;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Number of game bans";
                f.Value = player.NumberOfGameBans;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Economy ban";
                f.Value = player.EconomyBan;
            })
            .Build()).ConfigureAwait(false);
        }

        [Command("steamprofile")]
        [Summary("Returns the URL to the steam profile of the user")]
        public async Task SteamProfile(string name = null)
            => await ReplyAsync("https://steamcommunity.com/profiles/" +
                    (await _steamUser.ResolveVanityUrlAsync(name ?? Context.User.Username).ConfigureAwait(false)).Data
                ).ConfigureAwait(false);
    }
}
