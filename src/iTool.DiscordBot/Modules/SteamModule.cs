using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;

namespace iTool.DiscordBot.Modules
{
    public sealed class SteamModule : ModuleBase
    {
        private readonly Settings _settings;
        private readonly ISteamUser _steamUser;

        public SteamModule(Settings settings, ISteamUser steamUser)
        {
            _settings = settings;
            _steamUser = steamUser;
        }

        internal async Task<ulong> ResolveVanityURLInternal(string name)
        {
            var res = await _steamUser.ResolveVanityUrlAsync(name ?? Context.User.Username).ConfigureAwait(false);
            return res.Data;
        }

        [Command("vanityurl")]
        [Alias("resolvevanityurl")]
        [Summary("Returns the steamID64 of the user")]
        public async Task ResolveVanityURL(string name = null)
        {
            ulong steamId = await ResolveVanityURLInternal(name).ConfigureAwait(false);
            await ReplyAsync(steamId.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
        }

        [Command("steam")]
        [Summary("Returns basic steam profile information")]
        public async Task PlayerSummaries(string name = null)
        {
            ulong steamId = await ResolveVanityURLInternal(name).ConfigureAwait(false);
            PlayerSummaryModel player = (await _steamUser.GetPlayerSummaryAsync(steamId).ConfigureAwait(false)).Data;

            await ReplyAsync(
                string.Empty,
                embed: new EmbedBuilder()
                {
                    Title = $"Player summary for {player.Nickname}",
                    Color = _settings.GetColor(),
                    ThumbnailUrl = player.AvatarMediumUrl,
                    Url = player.ProfileUrl
                }
                .AddField("SteamID", player.SteamId, true)
                .AddField("Nickname", player.Nickname, true)
                .AddField("Persona state", (PersonaState)player.ProfileState, true)
                .Build()).ConfigureAwait(false);
        }

        [Command("playerbans")]
        [Alias("getplayerbans")]
        [Summary("Returns Community, VAC, and Economy ban statuses for given players")]
        public async Task PlayerBans(string name = null)
        {
            ulong steamId = await ResolveVanityURLInternal(name).ConfigureAwait(false);
            var res = await _steamUser.GetPlayerBansAsync(steamId).ConfigureAwait(false);
            PlayerBansModel player = res.Data.FirstOrDefault();

            await ReplyAsync(
                string.Empty,
                embed: new EmbedBuilder()
                {
                    Title = "Community, VAC, and Economy ban statuses",
                    Color = _settings.GetColor(),
                }
                .AddField("SteamID", player.SteamId, true)
                .AddField("CommunityBanned", player.CommunityBanned, true)
                .AddField("VACBanned", player.VACBanned, true)
                .AddField("Number of VAC bans", player.NumberOfVACBans, true)
                .AddField("Days since last ban", player.DaysSinceLastBan, true)
                .AddField("Number of game bans", player.NumberOfGameBans, true)
                .AddField("Economy ban", player.EconomyBan, true)
                .Build()).ConfigureAwait(false);
        }

        [Command("steamprofile")]
        [Summary("Returns the URL to the steam profile of the user")]
        public async Task SteamProfile(string name = null)
        {
            ulong steamId = await ResolveVanityURLInternal(name).ConfigureAwait(false);
            await ReplyAsync("https://steamcommunity.com/profiles/" + steamId).ConfigureAwait(false);
        }
    }
}
