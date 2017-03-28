using BfStats;
using BfStats.BfH;
using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class BfH : ModuleBase
    {
        BfHStatsClient Client;
        public BfH(BfHStatsClient statsClient) => this.Client = statsClient;

        [Command("bfhonlineplayers")]
        [Summary("Returns the amount of online players for Battlefield Hardline")]
        public async Task GetOnlinePlayers()
        {
            OnlinePlayers players = await Client.GetOnlinePlayers();

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Online players for BfH",
                Color = new Color((uint)Program.Settings.Color),
                ThumbnailUrl = "https://eaassets-a.akamaihd.net/battlelog/bb/bfh/logos/bfh-logo-670296c4.png"
            }
            .AddField(f =>
            {
                f.Name = "Total";
                f.Value = $"- Count: {players.PC.Count + players.PS3.Count + players.PS4.Count + players.Xbox.Count + players.XOne.Count}" + Environment.NewLine +
                    $"- Peak24: {players.PC.Peak24 + players.PS3.Peak24 + players.PS4.Peak24 + players.Xbox.Peak24 + players.XOne.Peak24}";
            })
            .AddField(f =>
            {
                f.Name = "PC";
                f.Value = $"- Count: {players.PC.Count}" + Environment.NewLine +
                    $"- Peak24: {players.PC.Peak24}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "PS3";
                f.Value = $"- Count: {players.PS3.Count}" + Environment.NewLine +
                    $"- Peak24: {players.PS3.Peak24}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "PS4";
                f.Value = $"- Count: {players.PS4.Count}" + Environment.NewLine +
                    $"- Peak24: {players.PS4.Peak24}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Xbox 360";
                f.Value = $"- Count: {players.Xbox.Count}" + Environment.NewLine +
                    $"- Peak24: {players.Xbox.Peak24}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Xbox One";
                f.Value = $"- Count: {players.XOne.Count}" + Environment.NewLine +
                    $"- Peak24: {players.XOne.Peak24}";
            }));
        }

        [Command("bfhstats")]
        [Alias("bfhplayerinfo")]
        [Summary("Returns data about a player")]
        public async Task GetPlayerInfo(string name = null, Platform platform = Platform.pc)
        {
            if (name == null) { name = Context.User.Username; }

            PlayerInfo playerInfo = await Client.GetPlayerInfo(platform, name);

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Battlefield Hardline stats for {playerInfo.Player.Name}",
                Color = new Color((uint)Program.Settings.Color),
                ThumbnailUrl = "https://eaassets-a.akamaihd.net/battlelog/bb/bfh/logos/bfh-logo-670296c4.png",
                Url = playerInfo.Player.BattlelogUser,
                Footer = new EmbedFooterBuilder()
                    {
                        Text = "Powered by bfhstats.com",
                    }
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = playerInfo.Player.UserName;
                f.Value = $"- Tag: {playerInfo.Player.Tag}" + Environment.NewLine +
                    $"- Country: {playerInfo.Player.CountryName}, {playerInfo.Player.Country}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Time played";
                f.Value = Math.Round(playerInfo.Player.TimePlayed.TotalHours, 2) + " hours";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Operator";
                f.Value = $"- Score: {playerInfo.Stats.Kits.Operator.Score}" + Environment.NewLine +
                    $"- SPM: {Math.Round(playerInfo.Stats.Kits.Operator.ScorePerMinute, 2)}" + Environment.NewLine +
                    $"- Stars: {playerInfo.Stats.Kits.Operator.Stars}" + Environment.NewLine +
                    $"- Time: {Math.Round(playerInfo.Stats.Kits.Operator.Time.TotalHours, 2)} hours";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Mechanic";
                f.Value = $"- Score: {playerInfo.Stats.Kits.Mechanic.Score}" + Environment.NewLine +
                    $"- SPM: {Math.Round(playerInfo.Stats.Kits.Mechanic.ScorePerMinute, 2)}" + Environment.NewLine +
                    $"- Stars: {playerInfo.Stats.Kits.Mechanic.Stars}" + Environment.NewLine +
                    $"- Time: {Math.Round(playerInfo.Stats.Kits.Mechanic.Time.TotalHours, 2)} hours";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Enforcer";
                f.Value = $"- Score: {playerInfo.Stats.Kits.Enforcer.Score}" + Environment.NewLine +
                    $"- SPM: {Math.Round(playerInfo.Stats.Kits.Enforcer.ScorePerMinute, 2)}" + Environment.NewLine +
                    $"- Stars: {playerInfo.Stats.Kits.Enforcer.Stars}" + Environment.NewLine +
                    $"- Time: {Math.Round(playerInfo.Stats.Kits.Enforcer.Time.TotalHours, 2)} hours";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Professional";
                f.Value = $"- Score: {playerInfo.Stats.Kits.Professional.Score}" + Environment.NewLine +
                    $"- SPM: {Math.Round(playerInfo.Stats.Kits.Professional.ScorePerMinute, 2)}" + Environment.NewLine +
                    $"- Stars: {playerInfo.Stats.Kits.Professional.Stars}" + Environment.NewLine +
                    $"- Time: {Math.Round(playerInfo.Stats.Kits.Professional.Time.TotalHours, 2)} hours";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Commander";
                f.Value = $"- Score: {playerInfo.Stats.Kits.Commander.Score}" + Environment.NewLine +
                    $"- SPM: {Math.Round(playerInfo.Stats.Kits.Commander.ScorePerMinute, 2)}" + Environment.NewLine +
                    $"- Stars: {playerInfo.Stats.Kits.Commander.Stars}" + Environment.NewLine +
                    $"- Time: {Math.Round(playerInfo.Stats.Kits.Commander.Time.TotalHours, 2)} hours";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Last Update";
                f.Value = playerInfo.Player.DateUpdate.ToString();
            }));
        }
    }
}
