using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using iTool.DiscordBot.Steam;

namespace iTool.DiscordBot.Modules
{
    public class CSGO : ModuleBase
    {
        private Settings _settings;
        private readonly SteamAPI _client;

        public CSGO(Settings settings, SteamAPI steamapi)
        {
            if (settings.SteamKey.IsNullOrEmpty())
            {
                throw new Exception("No SteamKey found.");
            }

            _settings = settings;
            _client = steamapi;
        }

        [Command("csgostats")]
        [Summary("Returns the CS:GO stats of the player")]
        public async Task CSGOStats(string name = null)
        {
            if (name == null) { name = Context.User.Username; }

            Dictionary<string, int> dict = (await _client.GetUserStatsForGame(730, await _client.ResolveVanityURL(name))).Stats
                                                .ToDictionary(x => x.Name, x => x.Value);

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
            if (name == null) { name = Context.User.Username; }

            Dictionary<string, int> dict = (await _client.GetUserStatsForGame(730, await _client.ResolveVanityURL(name))).Stats
                                                .ToDictionary(x => x.Name, x => x.Value);

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
