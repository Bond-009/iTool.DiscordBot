using Battlelog;
using Battlelog.BfH;
using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Guild : ModuleBase
    {
        GuildSettingsDatabase db;
        Settings settings;

        public Guild(Settings settings)
        {
            if (!settings.GuildSpecificSettings)
            {
                throw new Exception("The owner of this bot needs to enable guild specific settings for this module.");
            }
            this.settings = settings;
            db = new GuildSettingsDatabase();
            db.Database.EnsureCreated();
        }

        [Command("prefix")]
        [Summary("Returns the current prefix")]
        public async Task Prefix()
        {
            GuildSettings guildSettings = await db.GetSettingsAsync(Context.Guild.Id);

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Current prefix",
                Color = settings.GetColor(),
                Description = string.IsNullOrEmpty(guildSettings.Prefix) ? settings.Prefix : guildSettings.Prefix,
            });
        }

        [Command("prefix")]
        [Summary("Sets the current prefix")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task Prefix(string prefix)
        {
            GuildSettings guildSettings = await db.GetSettingsAsync(Context.Guild.Id);
            guildSettings.Prefix = prefix;
            await db.UpdateSettings(guildSettings);

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Prefix",
                Color = settings.GetColor(),
                Description = $"Changed to prefix for this server to {prefix}.",
            });
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}