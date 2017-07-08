using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Models;

namespace iTool.DiscordBot.Modules
{
    public class CSGOModule : ModuleBase
    {
        private readonly Settings _settings;
        private readonly ISteamUser _steamUser;
        private readonly ISteamUserStats _steamUserStats;

        public CSGOModule(Settings settings, ISteamUser steamUser, ISteamUserStats steamUserStats)
        {
            _settings = settings;
            _steamUser = steamUser;
            _steamUserStats = steamUserStats;
        }

        [Command("csgostats")]
        [Summary("Returns the CS:GO stats of the player")]
        public async Task CSGOStats(string name = null)
        {
            if (name == null) { name = Context.User.Username; }

            Dictionary<string, uint> dict = (await _steamUserStats.GetUserStatsForGameAsync(
                                                (await _steamUser.ResolveVanityUrlAsync(name)).Data,
                                                730)
                                            ).Data.Stats.ToDictionary(x => x.Name, x => x.Value);

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"CS:GO stats for {name}",
                Color = _settings.GetColor()
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Kills";
                f.Value = dict["total_kills"].ToString();
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Deaths";
                f.Value = dict["total_deaths"].ToString();
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "K/D";
                f.Value = Math.Round((double)dict["total_kills"] / dict["total_deaths"], 2).ToString();
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Headshots";
                f.Value = Math.Round(100.0 / dict["total_kills"] * dict["total_kills_headshot"], 2) + "%";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Accuracy";
                f.Value = Math.Round(100.0 / dict["total_shots_fired"] * dict["total_shots_hit"], 2) + "%";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Playtime";
                f.Value = (dict["total_time_played"] / 60 / 60) + " hours";
            }));
        }

        [Command("csgolastmatch")]
        [Summary("Returns stats of the player's last CS:GO match")]
        public async Task CSGOLastMatch(string name = null)
        {
            if (name == null) { name = Context.User.Username; }

            Dictionary<string, uint> dict = (await _steamUserStats.GetUserStatsForGameAsync(
                                                (await _steamUser.ResolveVanityUrlAsync(name)).Data,
                                                730)
                                            ).Data.Stats.ToDictionary(x => x.Name, x => x.Value);

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Last match CS:GO stats for {name}",
                Color = _settings.GetColor(),
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Kills";
                f.Value = dict["last_match_kills"].ToString();
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Deaths";
                f.Value = dict["last_match_deaths"].ToString();
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "K/D";
                f.Value = Math.Round((double)dict["last_match_kills"] / dict["last_match_deaths"], 2).ToString();
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "MVP";
                f.Value = dict["last_match_mvps"].ToString();
            }));
        }
    }
}
