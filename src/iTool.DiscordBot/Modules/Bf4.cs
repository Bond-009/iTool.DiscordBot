using Battlelog;
using Battlelog.Bf4;
using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Bf4 : ModuleBase, IDisposable
    {
        Bf4Client client;
        BfPlayerDatabase db;
        Settings settings;

        public Bf4(Bf4Client bfhClient, Settings settings)
        {
            this.client = bfhClient;
            this.settings = settings;
            db = new BfPlayerDatabase();
            db.Database.EnsureCreated();
        }

        [Command("bf4stats")]
        [Summary("Returns the Battlefield 4 stats of the player")]
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
                        ThumbnailUrl = "https://eaassets-a.akamaihd.net/bl-cdn/cdnprefix/production-283-20170323/public/base/bf4/header-logo-bf4.png"
                    });
                    return;
                }
            }

            DetailedStats stats = await client.GetDetailedStatsAsync(platform, personaID.Value);

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Battlefield 4 stats for {name}",
                Color = settings.GetColor(),
                ThumbnailUrl = "https://eaassets-a.akamaihd.net/bl-cdn/cdnprefix/production-283-20170323/public/base/bf4/header-logo-bf4.png",
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Rank";
                f.Value = $"- Rank: {stats.GeneralStats.Rank}" + Environment.NewLine +
                        $"- Score per minute: {stats.GeneralStats.ScorePerMinute}" + Environment.NewLine +
                        $"- Total score: {stats.GeneralStats.Score}";
            })
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
                f.Name = "K/D";
                f.Value = $"- K/D ratio: {stats.GeneralStats.KDRatio}" + Environment.NewLine +
                        $"- Kills: {stats.GeneralStats.Kills}" + Environment.NewLine +
                        $"- Deaths: {stats.GeneralStats.Deaths}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Misc";
                f.Value = $"- Accuracy: {Math.Round(stats.GeneralStats.Accuracy, 2)}%" + Environment.NewLine +
                        $"- Dogtags Taken: {stats.GeneralStats.DogtagsTaken}" + Environment.NewLine +
                        $"- Time played: {Math.Round(stats.GeneralStats.TimePlayed.TotalHours, 2)} hours";
            })
            );
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
