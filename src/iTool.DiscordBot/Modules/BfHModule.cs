using System;
using System.Threading.Tasks;
using Battlelog;
using Battlelog.BfH;
using Discord;
using Discord.Commands;

namespace iTool.DiscordBot.Modules
{
    public sealed class BfHModule : ModuleBase, IDisposable
    {
        private static BfHClient _client = new BfHClient();
        private readonly BfPlayerDatabase _db = new BfPlayerDatabase();
        private readonly Settings _settings;

        public BfHModule(Settings settings)
        {
            _settings = settings;

            _db.Database.EnsureCreated();
        }

        [Command("bfhstats")]
        [Summary("Returns the Battlefield Hardline stats of the player")]
        public async Task BfHStats(string name = null, Platform platform = Platform.PC, string platformSpecificName = null)
        {
            name ??= Context.User.Username;

            long? personaID = await _db.GetPersonaIDAsync(name).ConfigureAwait(false);

            if (personaID == null)
            {
                personaID = await _client.GetPersonaID(name, platform, platformSpecificName).ConfigureAwait(false);

                if (personaID != null)
                {
                    await _db.SavePersonaIDAsync(name, personaID.Value).ConfigureAwait(false);
                }
                else
                {
                    await ReplyAsync(string.Empty, embed: new EmbedBuilder()
                    {
                        Title = $"No player found",
                        Color = _settings.GetErrorColor(),
                        Description = "No player was found with that name.",
                        ThumbnailUrl = "https://eaassets-a.akamaihd.net/battlelog/bb/bfh/logos/bfh-logo-670296c4.png"
                    }.Build()).ConfigureAwait(false);
                    return;
                }
            }

            DetailedStats stats = await _client.GetDetailedStatsAsync(personaID.Value, platform);

            await ReplyAsync(string.Empty, embed: new EmbedBuilder()
            {
                Title = $"Battlefield Hardline stats for {name}",
                Color = _settings.GetColor(),
                ThumbnailUrl = "https://eaassets-a.akamaihd.net/battlelog/bb/bfh/logos/bfh-logo-670296c4.png"
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Rank";
                f.Value = $"**Rank**: {stats.GeneralStats.Rank}\n" +
                        $"- **Cash per minute**: {stats.GeneralStats.ScorePerMinute}\n" +
                        $"- **Total cash**: {stats.GeneralStats.Score}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Win / loss";
                f.Value = $"- **W/L ratio**: {Math.Round((double)stats.GeneralStats.Wins / stats.GeneralStats.Losses, 2)}\n" +
                        $"- **Wins**: {stats.GeneralStats.Wins}\n" +
                        $"- **Losses**: {stats.GeneralStats.Losses}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "K/D";
                f.Value = $"- **K/D ratio**: {stats.GeneralStats.KDRatio}\n" +
                        $"- **Kills**: {stats.GeneralStats.Kills}\n" +
                        $"- **Deaths**: {stats.GeneralStats.Deaths}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Misc";
                f.Value = $"- **Accuracy**: {Math.Round(stats.GeneralStats.Accuracy, 2)}%\n" +
                        $"- **Dogtags Taken**: {stats.GeneralStats.DogtagsTaken}\n" +
                        $"- **Time played**: {Math.Round(stats.GeneralStats.TimePlayed.TotalHours, 2)} hours";
            })
            .Build()).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
