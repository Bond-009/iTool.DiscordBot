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
            if (player == null)
            {
                await ReplyAsync("", embed: new EmbedBuilder()
                {
                    Title = $"No player found",
                    Color = settings.GetErrorColor(),
                    Description = "No player was found matching those criteria."
                });
                return;
            }

            EmbedBuilder b = new EmbedBuilder()
            {
                Title = $"HOTS player summary for {player.Name}",
                Color = settings.GetColor(),
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
                    f.Value = $"**LeagueID**: {ranking.LeagueID}\n" +
                        $"- **LeagueRank**: {ranking.LeagueRank}\n" +
                        $"- **CurrentMMR**: {ranking.CurrentMMR}";
                });
            }
            await ReplyAsync("", embed: b);
        }

        [Command("hotsstats")]
        [Summary("Returns the HOTS stats of the player")]
        public async Task HOTSStats(int playerID)
        {
            Player player = await client.GetPlayerSummary(playerID);
            if (player == null)
            {
                await ReplyAsync("", embed: new EmbedBuilder()
                {
                    Title = $"No player found",
                    Color = settings.GetErrorColor(),
                    Description = "No player was found matching those criteria."
                });
                return;
            }

            EmbedBuilder b = new EmbedBuilder()
            {
                Title = $"HOTS player summary for {player.Name}",
                Color = settings.GetColor(),
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
                    f.Value = $"- **LeagueID**: {ranking.LeagueID}\n" +
                        $"- **LeagueRank**: {ranking.LeagueRank}\n" +
                        $"- **CurrentMMR**: {ranking.CurrentMMR}";
                });
            }
            await ReplyAsync("", embed: b);
        }
    }
}
