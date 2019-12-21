using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using HOTSLogs;

namespace iTool.DiscordBot.Modules
{
    public class HOTSModule : ModuleBase
    {
        private static HOTSLogsClient _client;
        private readonly Settings _settings;

        public HOTSModule(Settings settings)
        {
            if (_client == null) _client = new HOTSLogsClient();
            _settings = settings;
        }

        [Command("hotsstats")]
        [Summary("Returns the HOTS stats of the player")]
        public async Task HOTSStats(Region region, string battleTag)
        {
            Player player = await _client.GetPlayerSummary(region, battleTag);
            if (player == null)
            {
                await ReplyAsync("", embed: new EmbedBuilder()
                {
                    Title = $"No player found",
                    Color = _settings.GetErrorColor(),
                    Description = "No player was found matching those criteria."
                }.Build());
                return;
            }

            EmbedBuilder b = new EmbedBuilder()
            {
                Title = $"HOTS player summary for {player.Name}",
                Color = _settings.GetColor(),
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
            await ReplyAsync("", embed: b.Build());
        }

        [Command("hotsstats")]
        [Summary("Returns the HOTS stats of the player")]
        public async Task HOTSStats(int playerID)
        {
            Player player = await _client.GetPlayerSummary(playerID);
            if (player == null)
            {
                await ReplyAsync("", embed: new EmbedBuilder()
                {
                    Title = $"No player found",
                    Color = _settings.GetErrorColor(),
                    Description = "No player was found matching those criteria."
                }.Build());
                return;
            }

            EmbedBuilder b = new EmbedBuilder()
            {
                Title = $"HOTS player summary for {player.Name}",
                Color = _settings.GetColor(),
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
            await ReplyAsync("", embed: b.Build());
        }
    }
}
