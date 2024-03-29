using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using HOTSLogs;

namespace iTool.DiscordBot.Modules
{
    public sealed class HOTSModule : ModuleBase
    {
        private static HOTSLogsClient _client;
        private readonly Settings _settings;

        public HOTSModule(Settings settings)
        {
            _client ??= new HOTSLogsClient();
            _settings = settings;
        }

        [Command("hotsstats")]
        [Summary("Returns the HOTS stats of the player")]
        public async Task HOTSStats(Region region, string battleTag)
        {
            Player player = await _client.GetPlayerSummary(region, battleTag).ConfigureAwait(false);
            if (player == null)
            {
                await ReplyAsync(
                    string.Empty,
                    embed: new EmbedBuilder()
                    {
                        Title = $"No player found",
                        Color = _settings.GetErrorColor(),
                        Description = "No player was found matching those criteria."
                    }.Build()).ConfigureAwait(false);
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
            .AddField("PlayerID", player.PlayerID.ToString(), true)
            .AddField("Name", player.Name, true);

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

            await ReplyAsync(string.Empty, embed: b.Build()).ConfigureAwait(false);
        }

        [Command("hotsstats")]
        [Summary("Returns the HOTS stats of the player")]
        public async Task HOTSStats(int playerID)
        {
            Player player = await _client.GetPlayerSummary(playerID).ConfigureAwait(false);
            if (player == null)
            {
                await ReplyAsync(
                    string.Empty,
                    embed: new EmbedBuilder()
                    {
                        Title = $"No player found",
                        Color = _settings.GetErrorColor(),
                        Description = "No player was found matching those criteria."
                    }.Build()).ConfigureAwait(false);
                return;
            }

            EmbedBuilder b = new EmbedBuilder()
            {
                Title = $"HOTS player summary for {player.Name}",
                Color = _settings.GetColor(),
                Url = $"https://www.hotslogs.com/Player/Profile?PlayerID={player.PlayerID}",
                ThumbnailUrl = "https://eu.battle.net/heroes/static/images/logos/logo.png"
            }
            .AddField("PlayerID", player.PlayerID.ToString(), true)
            .AddField("Name", player.Name, true);

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

            await ReplyAsync(string.Empty, embed: b.Build()).ConfigureAwait(false);
        }
    }
}
