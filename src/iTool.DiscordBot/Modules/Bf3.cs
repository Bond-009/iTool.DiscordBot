using Battlelog;
using Battlelog.Bf3;
using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Bf3 : ModuleBase
    {
        Bf3Client client;
        BfPlayerDatabase db;
        Settings settings;

        public Bf3(Bf3Client bfhClient, Settings settings)
        {
            this.client = bfhClient;
            this.settings = settings;
            db = new BfPlayerDatabase();
            db.Database.EnsureCreated();
        }

        [Command("bf3stats")]
        [Summary("Returns the Battlefield 3 stats of the player")]
        public async Task Bf4Stats(string name = null, Platform platform = Platform.PC)
        {
            if (name == null) { name = Context.User.Username; }

            long? personaID = await db.GetPersonaIDAsync(name);

            if (personaID == null)
            {
                personaID = await client.GetPersonaID(name);

                if (personaID != null)
                {
                    await db.SavePersonaIDAsync(name, personaID.Value);
                }
                else
                {
                    await ReplyAsync("", embed: new EmbedBuilder()
                    {
                        Title = $"No player found",
                        Color = settings.GetErrorColor(),
                        Description = "No player was found with that name.",
                        ThumbnailUrl = "https://eaassets-a.akamaihd.net/bl-cdn/cdnprefix/production-283-20170323/public/base/bf3/bf3-logo-m.png"
                    });
                    return;
                }
            }

            Stats stats = await client.GetStatsAsync(platform, personaID.Value);

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Battlefield 3 stats for {name}",
                Color = settings.GetColor(),
                ThumbnailUrl = "https://eaassets-a.akamaihd.net/bl-cdn/cdnprefix/production-283-20170323/public/base/bf3/bf3-logo-m.png",
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Rank";
                f.Value = $"- Rank: {stats.OverviewStats.Rank}" + Environment.NewLine +
                        $"- Score per minute: {stats.OverviewStats.ScorePerMinute}" + Environment.NewLine +
                        $"- Total score: {stats.OverviewStats.Score}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Win / loss";
                f.Value = $"- W/L ratio: {stats.OverviewStats.WLRatio}%" + Environment.NewLine +
                        $"- Wins: {stats.OverviewStats.Wins}" + Environment.NewLine +
                        $"- Losses: {stats.OverviewStats.Losses}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "K/D";
                f.Value = $"- K/D ratio: {Math.Round((double)stats.OverviewStats.Kills / stats.OverviewStats.Deaths, 2)}" + Environment.NewLine +
                        $"- Kills: {stats.OverviewStats.Kills}" + Environment.NewLine +
                        $"- Deaths: {stats.OverviewStats.Deaths}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Misc";
                f.Value = $"- Accuracy: {Math.Round(stats.OverviewStats.Accuracy, 2)}%" + Environment.NewLine +
                        $"- Dogtags Taken: {stats.OverviewStats.DogtagsTaken}" + Environment.NewLine +
                        $"- Time played: {Math.Round(stats.OverviewStats.TimePlayed.TotalHours, 2)} hours";
            })
            );
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
