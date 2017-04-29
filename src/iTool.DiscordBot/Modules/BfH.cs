using Battlelog;
using Battlelog.BfH;
using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class BfH : ModuleBase
    {
        DependencyMap depMap;
        BfHClient client;

        public BfH(DependencyMap map)
        {
            this.depMap = map;
            this.client = depMap.Get<BfHClient>();
        }

        [Command("bfhstats")]
        [Alias("bfhplayerinfo")]
        [Summary("Returns data about a player")]
        public async Task GetPlayerInfo(string name = null, Platform platform = Platform.PC)
        {
            if (name == null) { name = Context.User.Username; }

            long? personaID = await client.GetPersonaID(name);
            if (personaID == null)
            {
                await ReplyAsync("", embed: new EmbedBuilder()
                {
                    Title = $"No player found",
                    Color = new Color((uint)depMap.Get<Settings>().ErrorColor),
                    ThumbnailUrl = "https://eaassets-a.akamaihd.net/battlelog/bb/bfh/logos/bfh-logo-670296c4.png"
                });
                return;
            }

            DetailedStats stats = await client.GetDetailedStatsAsync(platform, personaID.Value);

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Battlefield Hardline stats for {name}",
                Color = new Color((uint)depMap.Get<Settings>().Color),
                ThumbnailUrl = "https://eaassets-a.akamaihd.net/battlelog/bb/bfh/logos/bfh-logo-670296c4.png",
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Win / loss";
                f.Value = $"- W/L ratio: {Math.Round((double)stats.GeneralStats.Wins / stats.GeneralStats.Losses, 2)}%" + Environment.NewLine +
                        $"- Wins: {stats.GeneralStats.Wins}" + Environment.NewLine +
                        $"- Losses: {stats.GeneralStats.Losses}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Score";
                f.Value = $"- Cash per minute: {stats.GeneralStats.CashPerMinute}" + Environment.NewLine +
                        $"- Total cash: {stats.GeneralStats.Score}" + Environment.NewLine +
                        $"- Time played: {Math.Round(stats.GeneralStats.TimePlayed.TotalHours, 2)} hours";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "K/D";
                f.Value = $"- K/D ratio: {stats.GeneralStats.KDRatio}" + Environment.NewLine +
                        $"- Kills: {stats.GeneralStats.Kills}" + Environment.NewLine +
                        $"- Deaths: {stats.GeneralStats.Deaths}";
            })
            );
        }
    }
}
