/*using BfStats.BfH;
using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class BfH : ModuleBase
    {
        [Command("bfhonlineplayers")]
        [Summary("Returns the amount of online players for Battlefield Hardline")]
        public async Task GetOnlinePlayers(string name = null)
        {
            BfHStatsClient client = new BfHStatsClient();
            OnlinePlayers players = await client.GetOnlinePlayers();

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Online players for BfH",
                Color = new Color((uint)Program.Settings.Color),
                ThumbnailUrl = "https://eaassets-a.akamaihd.net/battlelog/bb/bfh/logos/bfh-logo-670296c4.png"
            }
            .AddField(f =>
            {
                f.Name = "PC";
                f.Value = $"Count: {players.PC.Count}" + Environment.NewLine +
                    $"Peak24: {players.PC.Peak24}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "PS3";
                f.Value = $"Count: {players.PS3.Count}" + Environment.NewLine +
                    $"Peak24: {players.PS3.Peak24}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "PS4";
                f.Value = $"Count: {players.PS4.Count}" + Environment.NewLine +
                    $"Peak24: {players.PS4.Peak24}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Xbox 360";
                f.Value = $"Count: {players.XBox.Count}" + Environment.NewLine +
                    $"Peak24: {players.XBox.Peak24}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Xbox One";
                f.Value = $"Count: {players.XOne.Count}" + Environment.NewLine +
                    $"Peak24: {players.XOne.Peak24}";
            }));
        }
    }
}
*/