using Discord;
using Discord.Commands;
using iTool.DiscordBot.Steam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class CSGO : ModuleBase
    {
        [Command("csgostats")]
        [Summary("Returns the CS:GO stats of the player")]
        public async Task CSGOStats(string name = null)
        {
            if (string.IsNullOrEmpty(Program.Settings.SteamKey))
            {
                await Program.Log(new LogMessage(LogSeverity.Warning, nameof(CSGO), "No SteamKey found."));
                return;
            }

            if (name == null) { name = Context.User.Username; }

            Dictionary<string, int> dict = (await SteamAPI.GetUserStatsForGame(730, await SteamAPI.ResolveVanityURL(name))).Stats
                                                .ToDictionary(x => x.Name, x => x.Value);

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"CS:GO stats for {name}",
                Color = new Color((uint)Program.Settings.Color)
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
                f.Value = Math.Round((double)dict["total_kills"] / (double)dict["total_deaths"], 2).ToString();
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Headshots";
                f.Value = Math.Round(100 / (double)dict["total_kills"] * (double)dict["total_kills_headshot"], 2).ToString() + "%";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Accuracy";
                f.Value = Math.Round(100 / (double)dict["total_shots_fired"] * (double)dict["total_shots_hit"], 2).ToString() + "%";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Playtime";
                f.Value = (dict["total_time_played"] / 60 / 60).ToString() + " hours";
            }));
        }

        [Command("csgolastmatch")]
        [Summary("Returns stats of the player's last CS:GO match")]
        public async Task CSGOLastMatch(string name = null)
        {
            if (string.IsNullOrEmpty(Program.Settings.SteamKey))
            {
                await Program.Log(new LogMessage(LogSeverity.Warning, nameof(CSGO), "No SteamKey found."));
                return;
            }

            if (name == null) { name = Context.User.Username; }

            Dictionary<string, int> dict = (await SteamAPI.GetUserStatsForGame(730, await SteamAPI.ResolveVanityURL(name))).Stats
                                                .ToDictionary(x => x.Name, x => x.Value);

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Last match CS:GO stats for {name}",
                Color = new Color((uint)Program.Settings.Color),
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
                f.Value = Math.Round((double)dict["last_match_kills"] / (double)dict["last_match_deaths"], 2).ToString();
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