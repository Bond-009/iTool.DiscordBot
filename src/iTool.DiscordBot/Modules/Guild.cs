using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace iTool.DiscordBot.Modules
{
    public class Guild : ModuleBase, IDisposable
    {
        private readonly GuildSettingsDatabase _db;
        private Settings _settings;

        public Guild(Settings settings)
        {
            if (!settings.GuildSpecificSettings)
            {
                throw new Exception("The owner of this bot needs to enable guild specific settings for this module.");
            }

            _settings = settings;

            _db = new GuildSettingsDatabase();
        }

        [Command("prefix")]
        [Summary("Returns the current prefix")]
        public async Task Prefix()
        {
            GuildSettings guildSettings = await _db.GetSettingsAsync(Context.Guild.Id);

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Current prefix",
                Color = _settings.GetColor(),
                Description = string.IsNullOrEmpty(guildSettings.Prefix) ? _settings.Prefix : guildSettings.Prefix,
            });
        }

        [Command("prefix")]
        [Alias("setprefix", "prefix set")]
        [Summary("Sets the current prefix")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task Prefix(string prefix)
        {
            GuildSettings guildSettings = await _db.GetSettingsAsync(Context.Guild.Id);
            guildSettings.Prefix = prefix;
            await _db.UpdateSettings(guildSettings);

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Prefix",
                Color = _settings.GetColor(),
                Description = $"Changed to prefix for this server to {prefix}.",
            });
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
