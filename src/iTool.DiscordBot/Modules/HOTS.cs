using Discord;
using Discord.Commands;
using HOTSLogs;
using System;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class HOTS : ModuleBase
    {
        static HOTSLogsClient client;
        Settings settings;

        public HOTS(Settings settings)
        {
            if (client == null) client = new HOTSLogsClient();
            this.settings = settings;
        }

        [Command("hotsstats")]
        [Summary("Returns the HOTS stats of the player")]
        public async Task HOTSStats(Region region, string battleTag)
        {
            Player player = await client.GetPlayerSummary(region, battleTag);

            EmbedBuilder b = new EmbedBuilder()
            {
                Title = $"HOTS player summary for {player.Name}",
                Color = new Color((uint)settings.Color),
                Url = $"https://www.hotslogs.com/Player/Profile?PlayerID={player.PlayerID}",
                ThumbnailUrl = "https://eu.battle.net/heroes/static/images/logos/logo.png",
                Footer = new EmbedFooterBuilder()
                    {
                        Text = "Powered by hotslogs.com",
                    }
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "PlayerID";
                f.Value = player.PlayerID.ToString();
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Name";
                f.Value = player.Name;
            });

            foreach (Ranking ranking in player.LeaderboardRankings)
            {
                b.AddField(f =>
                {
                    f.IsInline = true;
                    f.Name = ranking.GameMode;
                    f.Value = $"- LeagueID: {ranking.LeagueID}" + Environment.NewLine +
                        $"- LeagueRank: {ranking.LeagueRank}" + Environment.NewLine +
                        $"- CurrentMMR: {ranking.CurrentMMR}";
                });
            }
            await ReplyAsync("", embed: b);
        }

        [Command("hotsstats")]
        [Summary("Returns the HOTS stats of the player")]
        public async Task HOTSStats(int playerID)
        {
            Player player = await client.GetPlayerSummary(playerID);

            EmbedBuilder b = new EmbedBuilder()
            {
                Title = $"HOTS player summary for {player.Name}",
                Color = new Color((uint)settings.Color),
                Url = $"https://www.hotslogs.com/Player/Profile?PlayerID={player.PlayerID}",
                ThumbnailUrl = "https://eu.battle.net/heroes/static/images/logos/logo.png"
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "PlayerID";
                f.Value = player.PlayerID.ToString();
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Name";
                f.Value = player.Name;
            });

            foreach (Ranking ranking in player.LeaderboardRankings)
            {
                b.AddField(f =>
                {
                    f.IsInline = true;
                    f.Name = ranking.GameMode;
                    f.Value = $"- LeagueID: {ranking.LeagueID}" + Environment.NewLine +
                        $"- LeagueRank: {ranking.LeagueRank}" + Environment.NewLine +
                        $"- CurrentMMR: {ranking.CurrentMMR}";
                });
            }
            await ReplyAsync("", embed: b);
        }
    }
}
