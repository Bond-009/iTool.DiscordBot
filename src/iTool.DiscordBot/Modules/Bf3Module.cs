using System;
using System.Threading.Tasks;
using Battlelog;
using Battlelog.Bf3;
using Discord;
using Discord.Commands;

namespace iTool.DiscordBot.Modules
{
    public class Bf3Module : ModuleBase, IDisposable
    {
        private static Bf3Client _client;
        private readonly BfPlayerDatabase _db = new BfPlayerDatabase();
        private Settings _settings;

        public Bf3Module(Settings settings)
        {
            _settings = settings;

            _db.Database.EnsureCreated();

            if (_client == null)
            { _client = new Bf3Client(); }
        }

        [Command("bf3stats")]
        [Summary("Returns the Battlefield 3 stats of the player")]
        public async Task Bf4Stats(string name = null, Platform platform = Platform.PC, string platformSpecificName = null)
        {
            if (name == null) { name = Context.User.Username; }

            long? personaID = await _db.GetPersonaIDAsync(name);

            if (personaID == null)
            {
                personaID = await _client.GetPersonaID(name, platform, platformSpecificName);

                if (personaID != null)
                {
                    await _db.SavePersonaIDAsync(name, personaID.Value);
                }
                else
                {
                    await ReplyAsync("", embed: new EmbedBuilder()
                    {
                        Title = $"No player found",
                        Color = _settings.GetErrorColor(),
                        Description = "No player was found with that name.",
                        ThumbnailUrl = "https://eaassets-a.akamaihd.net/bl-cdn/cdnprefix/production-283-20170323/public/base/bf3/bf3-logo-m.png"
                    });
                    return;
                }
            }

            Stats stats = await _client.GetStatsAsync(personaID.Value, platform);

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Battlefield 3 stats for {name}",
                Color = _settings.GetColor(),
                ThumbnailUrl = "https://eaassets-a.akamaihd.net/bl-cdn/cdnprefix/production-283-20170323/public/base/bf3/bf3-logo-m.png"
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Rank";
                f.Value = $"- **Rank**: {stats.OverviewStats.Rank}\n" +
                        $"- **Score per minute**: {stats.OverviewStats.ScorePerMinute}\n" +
                        $"- **Total score**: {stats.OverviewStats.Score}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Win / loss";
                f.Value = $"- **W/L ratio**: {stats.OverviewStats.WLRatio}%\n" + 
                        $"- **Wins**: {stats.OverviewStats.Wins}\n" + 
                        $"- **Losses**: {stats.OverviewStats.Losses}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "K/D";
                f.Value = $"- **K/D ratio**: {Math.Round((double)stats.OverviewStats.Kills / stats.OverviewStats.Deaths, 2)}\n" +
                        $"- **Kills**: {stats.OverviewStats.Kills}\n" +
                        $"- **Deaths**: {stats.OverviewStats.Deaths}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Misc";
                f.Value = $"- **Accuracy**: {Math.Round(stats.OverviewStats.Accuracy, 2)}%\n" +
                        $"- **Dogtags Taken**: {stats.OverviewStats.DogtagsTaken}\n" +
                        $"- **Time played**: {Math.Round(stats.OverviewStats.TimePlayed.TotalHours, 2)} hours";
            })
            );
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
